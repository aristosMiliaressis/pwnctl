using pwnctl.app;
using pwnctl.app.Assets;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Logging;
using pwnctl.app.Tasks.Entities;
using pwnctl.infra;
using pwnctl.infra.Aws;
using pwnctl.infra.Commands;
using pwnctl.infra.Queueing;
using pwnctl.infra.Repositories;

namespace pwnctl.svc
{
    public sealed class TaskConsumerService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly TaskQueueService _queueService = TaskQueueServiceFactory.Create();
        private readonly TaskDbRepository _taskRepo = new TaskDbRepository();
        private readonly AssetProcessor _processor = AssetProcessorFactory.Create();

        public TaskConsumerService(IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _hostApplicationLifetime.ApplicationStopping.Register(() => 
            {
                PwnInfraContext.Logger.Information(LogSinks.Console|LogSinks.Notification, $"{nameof(TaskConsumerService)} stoped.");
            });
            PwnInfraContext.Logger.Information(LogSinks.Console | LogSinks.Notification, $"{nameof(TaskConsumerService)} started.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var taskDTO = await _queueService.ReceiveAsync(stoppingToken);
                if (taskDTO == null)
                {
                    PwnInfraContext.Logger.Information("queue is empty");
                    Thread.Sleep(1000 * 60 * 3);
                    continue;
                }

                var taskEntry = await _taskRepo.GetEntryAsync(taskDTO.TaskId);

                // create a linked token that cancels the task when the timeout passes or 
                // when a SIGTERM is received due to an ECS scale in event
                var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                cts.CancelAfter(AwsConstants.MaxTaskTimeout * 1000);

                // Change the message visibility if the visibility window is exheeded
                // this allows us to keep a smaller visibility window without effecting 
                // the max task timeout.
                var timer = new System.Timers.Timer((AwsConstants.QueueVisibilityTimeoutInSec - 90) * 1000);
                timer.Elapsed += async (_, _) =>
                    await _queueService.ChangeMessageVisibilityAsync(taskDTO, AwsConstants.QueueVisibilityTimeoutInSec);
                timer.Start();

                try
                {
                    await ExecuteTaskAsync(taskEntry, cts.Token);
                }
                catch (Exception ex)
                {
                    PwnInfraContext.Logger.Exception(ex);

                    // return the task to the queue, if this occures to many times it will be put in the dead letter queue
                    await _queueService.ChangeMessageVisibilityAsync(taskDTO, 0);
                    continue;
                }
                finally
                {
                    timer.Dispose();
                }

                await _queueService.DequeueAsync(taskDTO);
            }

            _hostApplicationLifetime.StopApplication();
        }

        public async Task ExecuteTaskAsync(TaskEntry task, CancellationToken token = default)
        {
            task.Started();
            await _taskRepo.UpdateAsync(task);

            using (var process = await CommandExecutor.ExecuteAsync(task.Command, token: token))
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    var line = await process.StandardOutput.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                        continue;

                    await _processor.TryProcessAsync(line, task);

                    var pendingTasks = await _taskRepo.ListPendingAsync(token);
                    foreach (var t in pendingTasks)
                    {
                        t.Queued();
                        await _queueService.EnqueueAsync(new QueuedTaskDTO(t), token);
                        await _taskRepo.UpdateAsync(t);
                    }
                }

                string stderr = await process.StandardError.ReadToEndAsync();
                if (!string.IsNullOrEmpty(stderr))
                    PwnInfraContext.Logger.Error(stderr);

                task.Finished(process.ExitCode);
            }

            await _taskRepo.UpdateAsync(task);
        }
    }
}
