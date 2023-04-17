using pwnctl.app;
using pwnctl.app.Assets;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Notifications.Enums;
using pwnctl.infra;
using pwnctl.infra.Commands;
using pwnctl.infra.Queueing;
using pwnctl.infra.Repositories;
using System.Text;

namespace pwnctl.svc
{
    public sealed class TaskConsumerService : BackgroundService
    {
        private readonly AssetProcessor _processor = AssetProcessorFactory.Create();
        private readonly TaskQueueService _queueService = TaskQueueServiceFactory.Create();
        private readonly TaskDbRepository _taskRepo = new TaskDbRepository();

        public TaskConsumerService(IHostApplicationLifetime hostApplicationLifetime)
        {
            hostApplicationLifetime.ApplicationStopping.Register(() => 
            {
                PwnInfraContext.NotificationSender.Send($"{nameof(TaskConsumerService)} stoped.", NotificationTopic.status);
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PwnInfraContext.NotificationSender.Send($"{nameof(TaskConsumerService)} started.", NotificationTopic.status);

            while (!stoppingToken.IsCancellationRequested)
            {
                var taskDTO = await _queueService.ReceiveAsync<PendingTaskDTO>(stoppingToken);
                if (taskDTO == null)
                {
                    await ProcessOutputBatchAsync(stoppingToken);                    
                    continue;
                }

                await ExecuteTaskAsync(taskDTO, stoppingToken);
            }
        }

        private async Task ExecuteTaskAsync(PendingTaskDTO taskDTO, CancellationToken stoppingToken)
        {
            // create a linked token that cancels the task when the max task timeout 
            // passes or when a SIGTERM is received due to a scale in event
            var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            cts.CancelAfter(PwnInfraContext.Config.Worker.MaxTaskTimeout * 1000);

            // Change the message visibility if the visibility window is exheeded
            // this allows us to keep a smaller visibility window without effecting 
            // the max task timeout.
            var timer = new System.Timers.Timer((PwnInfraContext.Config.TaskQueue.VisibilityTimeout - 90) * 1000);
            timer.Elapsed += async (_, _) =>
                await _queueService.ChangeMessageVisibilityAsync(taskDTO, PwnInfraContext.Config.TaskQueue.VisibilityTimeout);
            timer.Start();

            try
            {
                var task = await _taskRepo.GetEntryAsync(taskDTO.TaskId);

                task.Started();
                await _taskRepo.UpdateAsync(task);

                (int exitCode, 
                StringBuilder stdout, 
                StringBuilder stderr) = await CommandExecutor.ExecuteAsync(task.Command, token: cts.Token);

                await _queueService.DequeueAsync(taskDTO);

                task.Finished(exitCode);
                await _taskRepo.UpdateAsync(task);

                timer.Dispose();

                if (!string.IsNullOrWhiteSpace(stderr.ToString()))
                    PwnInfraContext.Logger.Error(stderr.ToString());

                var lines = stdout.ToString()
                                .Split("\n")
                                .Where(l => !string.IsNullOrEmpty(l));
                
                OutputBatchDTO outputBatch = null;
                for (int s = 10, i = 0; true; i++)
                {
                    outputBatch = new()
                    {
                        TaskId = taskDTO.TaskId,
                        Lines = lines.Skip(i * s).Take(s).ToList()
                    };

                    if (!outputBatch.Lines.Any())
                        break;
                    
                    await _queueService.EnqueueAsync(outputBatch);
                }
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);

                // return the task to the queue, if this occures to many times,
                // the task will be put in the dead letter queue
                await _queueService.ChangeMessageVisibilityAsync(taskDTO, 0);
            }
        }

        private async Task ProcessOutputBatchAsync(CancellationToken stoppingToken)
        {
            var batchDTO = await _queueService.ReceiveAsync<OutputBatchDTO>(stoppingToken);
            if (batchDTO == null)
            {
                PwnInfraContext.Logger.Information("no work found");
                Thread.Sleep(1000 * 60 * 3);
                return;
            }

            try
            {
                var task = await _taskRepo.GetEntryAsync(batchDTO.TaskId);

                foreach (var line in batchDTO.Lines)
                {
                    PwnInfraContext.Logger.Debug("Processing: "+line);

                    await _processor.TryProcessAsync(line, task);

                    var pendingTasks = await _taskRepo.ListPendingAsync();
                    foreach (var t in pendingTasks)
                    {
                        await _queueService.EnqueueAsync(new PendingTaskDTO(t));
                        t.Queued();
                        await _taskRepo.UpdateAsync(t);
                    }
                }

                await _queueService.DequeueAsync(batchDTO);
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);

                // return the task to the queue, if this occures to many times,
                // the task will be put in the dead letter queue
                await _queueService.ChangeMessageVisibilityAsync(batchDTO, 0);
            }
        }
    }
}
