namespace pwnctl.exec;

using pwnctl.app;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Tasks.Enums;
using pwnctl.infra.Repositories;
using pwnctl.infra.Persistence;
using System.Text;
using pwnctl.infra;

public sealed class TaskExecutorService : LifetimeService
{
    private static readonly QueryRunner _queryRunner = new();
    private static readonly TaskDbRepository _taskRepo = new();
    private static bool _protectionState = false;

    public TaskExecutorService(IHostApplicationLifetime svcLifetime)
        : base(svcLifetime)
    {
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecutePendingTaskAsync(stoppingToken);
            } 
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);

                _protectionState = false;
                if (_protectionState)
                {
                    // disable scale in protection to allow scale in events
                    await ScaleInProtection.DisableAsync();
                    _protectionState = false;
                }
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
                PwnInfraContext.Logger.Information("task queue is empty, sleeping for 5 secs.");

                if (_protectionState) 
                {
                    // disable scale in protection to allow scale in events while waiting for task.
                    await ScaleInProtection.DisableAsync();
                    _protectionState = false;
                }

                await StopAsync(stoppingToken);
                return;
            }
        } 
        catch (OperationCanceledException)
        {
            return;
        }

        if (!_protectionState)
        {
            // setup scale in protection to prevent scale in events while the task is running
            await ScaleInProtection.EnableAsync();
            _protectionState = true;
        }

        // create a linked token that cancels the task when max 
        // task timeout is exheeded or if a scale in event occures
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        cts.CancelAfter((PwnInfraContext.Config.Worker.MaxTaskTimeout - 2) * 1000);

        // Change the message visibility if the visibility window is exheeded
        // this allows us to keep a smaller visibility window without effecting
        // the max task timeout.
        using var timer = new System.Timers.Timer((PwnInfraContext.Config.TaskQueue.VisibilityTimeout - 90) * 1000);
        timer.Elapsed += async (_, _) =>
            await PwnInfraContext.TaskQueueService.ChangeMessageVisibilityAsync(taskDTO, PwnInfraContext.Config.TaskQueue.VisibilityTimeout);
        timer.Start();

        var task = await _taskRepo.FindAsync(taskDTO.TaskId);
        if (task is null)
        {
            PwnInfraContext.Logger.Warning($"Task {taskDTO.TaskId} \"{taskDTO.Command}\" not found in database.");

            await PwnInfraContext.TaskQueueService.DequeueAsync(taskDTO);

            return;
        }

        bool succeeded = task.Started();
        if (!succeeded)
        {
            PwnInfraContext.Logger.Warning($"Invalid TaskRecord:{task.Id} state transition from {task.State} to {TaskState.RUNNING}.");

            // probably a deduplication issue, remove from queue and move on
            await PwnInfraContext.TaskQueueService.DequeueAsync(taskDTO);

            return;
        }

        succeeded = await _taskRepo.TryUpdateAsync(task);
        if (!succeeded)
        {
            PwnInfraContext.Logger.Warning($"failed to update task records #{task.Id} state to STARTED.");
        }

        StringBuilder stdin = null;
        if (task.Definition.StdinQuery is not null)
        {
            var (succedded, json) = await _queryRunner.TryRunAsync(task.Definition.StdinQuery);
            if (!succedded)
            {
                task.Failed();

                succeeded = await _taskRepo.TryUpdateAsync(task);
                if (!succeeded)
                {
                    PwnInfraContext.Logger.Warning($"failed to update task records #{task.Id} state to FAILED.");
                }

                await PwnInfraContext.TaskQueueService.DequeueAsync(taskDTO);

                return;
            }

            stdin = new(json);
        }

        int exitCode = 0;
        StringBuilder stdout = null, stderr = null;
        try
        {
            PwnInfraContext.Logger.Information("Running: " + task.Command);

            (exitCode, stdout, stderr) = await PwnInfraContext.CommandExecutor.ExecuteAsync(task.Command, stdin, token: cts.Token);

            task.Finished(exitCode, stderr.ToString());
        }
        catch (Exception ex)
        {
            PwnInfraContext.Logger.Exception(ex);

            task.Failed();

            succeeded = await _taskRepo.TryUpdateAsync(task);
            if (!succeeded)
            {
                PwnInfraContext.Logger.Warning($"failed to update task records #{task.Id} state to FAILED.");
            }

            // return the task to the queue, if this occures to many times,
            // the task will be put in the dead letter queue
            await PwnInfraContext.TaskQueueService.ChangeMessageVisibilityAsync(taskDTO, 0);

            return;          
        }

        succeeded = await _taskRepo.TryUpdateAsync(task);
        if (!succeeded)
        {
            PwnInfraContext.Logger.Warning($"failed to update task records #{task.Id} state to FINISHED.");
        }

        await PwnInfraContext.TaskQueueService.DequeueAsync(taskDTO);
        timer.Stop();

        var lines = stdout.ToString()
                        .Split("\n")
                        .Where(l => !string.IsNullOrEmpty(l));

        PwnInfraContext.Logger.Information($"Task: {taskDTO.TaskId} produced {lines.Count()} lines");

        var batches = OutputBatchDTO.FromLines(lines, task.Id);
        foreach (var batch in batches)
            await PwnInfraContext.TaskQueueService.EnqueueAsync(batch);
    }
}
