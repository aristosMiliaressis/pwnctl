using pwnctl.app;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Notifications.Enums;
using pwnctl.infra.Commands;
using pwnctl.infra.Queueing;
using pwnctl.infra.Repositories;
using System.Text;
using pwnctl.infra.Configuration;
using pwnctl.app.Tasks.Exceptions;

namespace pwnctl.exec
{
    public sealed class TaskExecutorService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private static readonly TaskDbRepository _taskRepo = new();
        private static System.Timers.Timer _timer = new();

        public TaskExecutorService(IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                PwnInfraContext.NotificationSender.SendAsync($"{nameof(TaskExecutorService)} stoped.", NotificationTopic.Status).Wait();
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await PwnInfraContext.NotificationSender.SendAsync($"{nameof(TaskExecutorService)}:{EnvironmentVariables.IMAGE_HASH} started.", NotificationTopic.Status);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ExecutePendingTaskAsync(stoppingToken);
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
            var taskDTO = await PwnInfraContext.TaskQueueService.ReceiveAsync<PendingTaskDTO>(stoppingToken);
            if (taskDTO is null)
            {
                PwnInfraContext.Logger.Information("no work found");
                Thread.Sleep(5000);
                return;
            }

            // create a linked token that cancels the task when the max task timeout
            // passes or when a SIGTERM is received due to a scale in event
            var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            cts.CancelAfter(PwnInfraContext.Config.Worker.MaxTaskTimeout * 1000);

            // Change the message visibility if the visibility window is exheeded
            // this allows us to keep a smaller visibility window without effecting
            // the max task timeout.
            _timer = new System.Timers.Timer((PwnInfraContext.Config.TaskQueue.VisibilityTimeout - 90) * 1000);
            _timer.Elapsed += async (_, _) =>
                await PwnInfraContext.TaskQueueService.ChangeMessageVisibilityAsync(taskDTO, PwnInfraContext.Config.TaskQueue.VisibilityTimeout);
            _timer.Start();

            var task = await _taskRepo.FindAsync(taskDTO.TaskId);
            if (task is null)
            {
                PwnInfraContext.Logger.Warning($"Task {taskDTO.TaskId} \"{taskDTO.Command}\" not found in database.");
                _timer.Stop();

                await PwnInfraContext.TaskQueueService.DequeueAsync(taskDTO);
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
                await PwnInfraContext.TaskQueueService.DequeueAsync(taskDTO);
                return;
            }

            try
            {
                await _taskRepo.UpdateAsync(task);

                (int exitCode,
                StringBuilder stdout,
                StringBuilder stderr) = await PwnInfraContext.CommandExecutor.ExecuteAsync(task.Command, token: cts.Token);

                task.Finished(exitCode, stderr.ToString());
                await _taskRepo.UpdateAsync(task);

                _timer.Stop();

                await PwnInfraContext.TaskQueueService.DequeueAsync(taskDTO);

                var lines = stdout.ToString()
                                .Split("\n")
                                .Where(l => !string.IsNullOrEmpty(l));

                PwnInfraContext.Logger.Debug($"Task: {taskDTO.TaskId} produced {lines.Count()}");

                var batches = OutputBatchDTO.FromLines(lines, task.Id);
                foreach (var batch in batches)
                    await PwnInfraContext.TaskQueueService.EnqueueAsync(batch);
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);

                task.Failed();
                await _taskRepo.UpdateAsync(task);

                _timer.Dispose();

                // return the task to the queue, if this occures to many times,
                // the task will be put in the dead letter queue
                await PwnInfraContext.TaskQueueService.ChangeMessageVisibilityAsync(taskDTO, 0);
            }
        }
    }
}
