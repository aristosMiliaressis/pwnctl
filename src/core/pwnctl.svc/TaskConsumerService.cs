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
using pwnctl.app.Operations;
using System.Security.Cryptography;

namespace pwnctl.svc
{
    public sealed class TaskConsumerService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly AssetProcessor _processor = AssetProcessorFactory.Create();
        private readonly TaskQueueService _queueService = TaskQueueServiceFactory.Create();
        private readonly TaskDbRepository _taskRepo = new();
        private readonly OperationInitializer _initializer = new(new OperationDbRepository(),
                                                                new AssetDbRepository(),
                                                                new TaskDbRepository(),
                                                                TaskQueueServiceFactory.Create());

        public TaskConsumerService(IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                PwnInfraContext.NotificationSender.Send($"{nameof(TaskConsumerService)} stoped.", NotificationTopic.status);
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PwnInfraContext.NotificationSender.Send($"{nameof(TaskConsumerService)} started.", NotificationTopic.status);

            if (int.TryParse(Environment.GetEnvironmentVariable("PWNCTL_Operation"), out int opId))
            {
                await _initializer.InitializeAsync(opId);
                _hostApplicationLifetime.StopApplication();
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var taskDTO = await _queueService.ReceiveAsync<PendingTaskDTO>(stoppingToken);
                if (taskDTO == null)
                {
                    await ProcessOutputBatchAsync(stoppingToken);
                    continue;
                }

                await ExecutePendingTaskAsync(taskDTO, stoppingToken);
            }
        }


        private async Task ExecutePendingTaskAsync(PendingTaskDTO taskDTO, CancellationToken stoppingToken)
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
                var task = await _taskRepo.FindAsync(taskDTO.TaskId);

                task.Started();
                await _taskRepo.UpdateAsync(task);

                (int exitCode,
                StringBuilder stdout,
                StringBuilder stderr) = await CommandExecutor.ExecuteAsync(task.Command, token: cts.Token);

                timer.Dispose();
                await _queueService.DequeueAsync(taskDTO);

                task.Finished(exitCode);
                await _taskRepo.UpdateAsync(task);

                if (!string.IsNullOrWhiteSpace(stderr.ToString()))
                    PwnInfraContext.Logger.Error(stderr.ToString());

                var lines = stdout.ToString()
                                .Split("\n")
                                .Where(l => !string.IsNullOrEmpty(l));

                OutputBatchDTO outputBatch = new(task.Id);
                for (int max = 10, i = 0; i < lines.Count(); i++)
                {
                    var line = lines.ElementAt(i);
                    if ((string.Join(",", outputBatch.Lines).Length + line.Length) < 8000)
                        outputBatch.Lines.Add(line);

                    if (outputBatch.Lines.Count == max || (string.Join(",", outputBatch.Lines).Length + line.Length) >= 8000)
                    {
                        using (MD5 md5 = MD5.Create())
                        {
                            var md5Bytes = md5.ComputeHash(Encoding.ASCII.GetBytes(string.Join(",", outputBatch)));
                            outputBatch.Metadata["MessageGroupId"] = Convert.ToHexString(md5Bytes);
                        }

                        await _queueService.EnqueueAsync(outputBatch);
                        outputBatch = new(task.Id);
                        outputBatch.Lines.Add(line);
                    }
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
                Thread.Sleep(1000 * 60);
                return;
            }

            // Change the message visibility if the visibility window is exheeded
            // this allows us to keep a smaller visibility window without effecting
            // the max task timeout.
            var timer = new System.Timers.Timer((PwnInfraContext.Config.TaskQueue.VisibilityTimeout - 90) * 1000);
            timer.Elapsed += async (_, _) =>
                await _queueService.ChangeMessageVisibilityAsync(batchDTO, PwnInfraContext.Config.TaskQueue.VisibilityTimeout);
            timer.Start();

            try
            {
                var task = await _taskRepo.FindAsync(batchDTO.TaskId);

                foreach (var line in batchDTO.Lines)
                {
                    PwnInfraContext.Logger.Debug("Processing: "+line);

                    await _processor.TryProcessAsync(line, task.Operation, task);
                }

                timer.Dispose();
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
