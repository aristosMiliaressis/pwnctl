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
using pwnctl.infra.Configuration;
using pwnctl.app.Tasks.Exceptions;

namespace pwnctl.svc
{
    public sealed class TaskConsumerService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private static readonly AssetProcessor _processor = AssetProcessorFactory.Create();
        private static readonly TaskQueueService _queueService = TaskQueueServiceFactory.Create();
        private static readonly TaskDbRepository _taskRepo = new();
        private static System.Timers.Timer _timer = new();
        private static readonly OperationInitializer _initializer = new(new OperationDbRepository(),
                                                                new AssetDbRepository(),
                                                                _taskRepo,
                                                                _queueService);

        public TaskConsumerService(IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                PwnInfraContext.NotificationSender.Send($"{nameof(TaskConsumerService)} stoped.", NotificationTopic.Status);
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PwnInfraContext.NotificationSender.Send($"{nameof(TaskConsumerService)}:{EnvironmentVariables.IMAGE_HASH} started.", NotificationTopic.Status);

            if (int.TryParse(Environment.GetEnvironmentVariable("PWNCTL_Operation"), out int opId))
            {
                await _initializer.InitializeAsync(opId);
                _hostApplicationLifetime.StopApplication();
                return;
            }

            var rng = new Random();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // favor output vs task processing at 80%
                    if (rng.Next(10) < 8)
                    {
                        await ExecutePendingTaskAsync(stoppingToken);
                        continue;
                    }
                    await ProcessOutputBatchAsync(stoppingToken);
                } 
                catch (Exception ex)
                {
                    PwnInfraContext.Logger.Exception(ex);
                }
                finally
                {
                    _timer.Dispose();
                }
            }
        }

        private async Task ExecutePendingTaskAsync(CancellationToken stoppingToken)
        {
            var taskDTO = await _queueService.ReceiveAsync<PendingTaskDTO>(stoppingToken);

            // create a linked token that cancels the task when the max task timeout
            // passes or when a SIGTERM is received due to a scale in event
            var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            cts.CancelAfter(PwnInfraContext.Config.Worker.MaxTaskTimeout * 1000);

            // Change the message visibility if the visibility window is exheeded
            // this allows us to keep a smaller visibility window without effecting
            // the max task timeout.
            _timer = new System.Timers.Timer((PwnInfraContext.Config.TaskQueue.VisibilityTimeout - 90) * 1000);
            _timer.Elapsed += async (_, _) =>
                await _queueService.ChangeMessageVisibilityAsync(taskDTO, PwnInfraContext.Config.TaskQueue.VisibilityTimeout);
            _timer.Start();

            var task = await _taskRepo.FindAsync(taskDTO.TaskId);
            if (task == null)
            {
                PwnInfraContext.Logger.Warning($"Task {taskDTO.TaskId} \"{taskDTO.Command}\" not found in database.");
                _timer.Stop();

                await _queueService.DequeueAsync(taskDTO);
                return;
            }

            try
            {
                task.Started();
            }
            catch (TaskStateException ex)
            {
                PwnInfraContext.Logger.Warning(ex.Message);
                _timer.Stop();

                // probably a deduplication issue, remove from queue and move on
                await _queueService.DequeueAsync(taskDTO);
                return;
            }

            try
            {
                await _taskRepo.UpdateAsync(task);

                (int exitCode,
                StringBuilder stdout,
                StringBuilder stderr) = await CommandExecutor.ExecuteAsync(task.Command, token: cts.Token);

                task.Finished(exitCode, stderr.ToString());
                await _taskRepo.UpdateAsync(task);

                _timer.Stop();

                await _queueService.DequeueAsync(taskDTO);

                var lines = stdout.ToString()
                                .Split("\n")
                                .Where(l => !string.IsNullOrEmpty(l));

                PwnInfraContext.Logger.Debug($"Task: {taskDTO.TaskId} produced {lines.Count()}");

                var batches = OutputBatchDTO.FromLines(lines, task.Id);
                foreach (var batch in batches)
                    await _queueService.EnqueueAsync(batch);
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);

                task.Failed();
                await _taskRepo.UpdateAsync(task);

                _timer.Dispose();

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
                return;
            }

            // Change the message visibility if the visibility window is exheeded
            // this allows us to keep a smaller visibility window without effecting
            // the max task timeout.
            _timer = new System.Timers.Timer((PwnInfraContext.Config.TaskQueue.VisibilityTimeout - 90) * 1000);
            _timer.Elapsed += async (_, _) =>
                await _queueService.ChangeMessageVisibilityAsync(batchDTO, PwnInfraContext.Config.TaskQueue.VisibilityTimeout);
            _timer.Start();

            try
            {
                foreach (var line in batchDTO.Lines)
                {
                    PwnInfraContext.Logger.Debug("Processing: " + line);

                    await _processor.TryProcessAsync(line, batchDTO.TaskId);
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
            finally
            {
                _timer.Stop();
            }
        }
    }
}
