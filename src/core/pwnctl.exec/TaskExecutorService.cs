
    using pwnctl.app;
    using pwnctl.app.Queueing.DTO;
    using pwnctl.app.Tasks.Enums;
    using pwnctl.app.Notifications.Enums;
    using pwnctl.infra.Repositories;
    using pwnctl.infra.Persistence;
    using System.Text;
    using pwnctl.infra.Configuration;

    public sealed class TaskExecutorService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private static readonly QueryRunner _queryRunner = new();
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
            await PwnInfraContext.NotificationSender.SendAsync($"{nameof(TaskExecutorService)}:{EnvironmentVariables.COMMIT_HASH} started.", NotificationTopic.Status);

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

                    // disable scale in protection to allow scale in events
                    await ScaleInProtection.DisnableAsync();
                }
            }
        }

        private async Task ExecutePendingTaskAsync(CancellationToken stoppingToken)
        {
            PendingTaskDTO taskDTO = null;
            try 
            {
                taskDTO = await PwnInfraContext.TaskQueueService.ReceiveAsync<PendingTaskDTO>(stoppingToken);
                if (taskDTO is null)
                {
                    PwnInfraContext.Logger.Information("no work found");
                    Thread.Sleep(5000);
                    return;
                }
            } 
            catch (TaskCanceledException)
            {
                // scale in event caught
                PwnInfraContext.Logger.Warning("Scale in event caught");
                _hostApplicationLifetime.StopApplication();
                return;
            }
            
            // setup scale in protection to prevent scale in events while the task is running
            await ScaleInProtection.EnableAsync();

            // create a linked token that cancels the task when the max task timeout
            // passes or when a SIGTERM is received due to a scale in event
            var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            cts.CancelAfter((PwnInfraContext.Config.Worker.MaxTaskTimeout - 2) * 1000);

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


            bool succeeded = task.Started();
            if (!succeeded)
            {
                PwnInfraContext.Logger.Warning($"Invalid TaskRecord:{task.Id} state transition from {task.State} to {TaskState.RUNNING}.");

                _timer.Stop();

                // probably a deduplication issue, remove from queue and move on
                await PwnInfraContext.TaskQueueService.DequeueAsync(taskDTO);
                return;
            }

            try
            {
                await _taskRepo.UpdateAsync(task);
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);

                _timer.Dispose();

                // return the task to the queue, if this occures to many times,
                // the task will be put in the dead letter queue
                await PwnInfraContext.TaskQueueService.ChangeMessageVisibilityAsync(taskDTO, 0);
            }

        try
        {
            StringBuilder? stdin = null;
            if (task.Definition.StdinQuery is not null)
            {
                var json = await _queryRunner.RunAsync(task.Definition.StdinQuery);

                stdin = new(json);
            }

            (int exitCode,
            StringBuilder stdout,
            StringBuilder stderr) = await PwnInfraContext.CommandExecutor.ExecuteAsync(task.Command, stdin, token: cts.Token);

                task.Finished(exitCode, stderr.ToString());

                await _taskRepo.UpdateAsync(task);

                await PwnInfraContext.TaskQueueService.DequeueAsync(taskDTO);
                _timer.Stop();

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

                _timer.Dispose();

                if (task.State == TaskState.RUNNING)
                {
                    // return the task to the queue, if this occures to many times,
                    // the task will be put in the dead letter queue
                    await PwnInfraContext.TaskQueueService.ChangeMessageVisibilityAsync(taskDTO, 0);

                    task.Failed();
                }
                else
                {
                    await PwnInfraContext.TaskQueueService.DequeueAsync(taskDTO);
                }

                await _taskRepo.UpdateAsync(task);
            }
        }
    }
}
