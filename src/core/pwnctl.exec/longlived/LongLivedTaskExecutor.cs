namespace pwnctl.exec;

using pwnctl.app;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Tasks.Enums;
using pwnctl.app.Common.Extensions;
using pwnctl.infra.Repositories;
using pwnctl.infra.Persistence;
using System.Text;
using pwnctl.infra;
using pwnctl.infra.Configuration;

public sealed class LongLivedTaskExecutor : LifetimeService
{
    private static readonly QueryRunner _queryRunner = new();
    private static readonly TaskDbRepository _taskRepo = new();
    private static bool _protectionState = false;

    public LongLivedTaskExecutor(IHostApplicationLifetime svcLifetime)
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
        var taskDTO = await PwnInfraContext.TaskQueueService.ReceiveAsync<LongLivedTaskDTO>(stoppingToken);
        if (taskDTO is null)
        {
            PwnInfraContext.Logger.Information("task queue is empty, exiting.");

            if (_protectionState)
            {
                // disable scale in protection to allow scale in events while waiting for task.
                await ScaleInProtection.DisableAsync();
                _protectionState = false;
            }

            return;
        }

        if (!_protectionState)
        {
            // setup scale in protection to prevent scale in events while the task is running
            await ScaleInProtection.EnableAsync();
            _protectionState = true;
        }

        // Change the message visibility if the visibility window is exheeded
        // this allows us to keep a smaller visibility window without effecting
        // the max task timeout.
        using var timer = new System.Timers.Timer((PwnInfraContext.Config.LongLivedTaskQueue.VisibilityTimeout - 30) * 1000);
        timer.Elapsed += async (_, _) =>
            await PwnInfraContext.TaskQueueService.ChangeMessageVisibilityAsync(taskDTO, PwnInfraContext.Config.LongLivedTaskQueue.VisibilityTimeout);
        timer.Start();

        var task = await _taskRepo.FindAsync(taskDTO.TaskId);
        if (task is null)
        {
            PwnInfraContext.Logger.Warning($"Task {taskDTO.TaskId} \"{taskDTO.Command}\" not found in database.");

            await PwnInfraContext.TaskQueueService.DequeueAsync(taskDTO);

            return;
        }

        bool succeeded = false;
        StringBuilder stdin = null;
        if (task.Definition.StdinQuery is not null)
        {
            var query = task.Definition.StdinQuery.Interpolate(task.Record.Asset);
            var (succedded, json) = await _queryRunner.TryRunAsync(query);
            if (!succedded)
            {
                task.Failed(null);

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

        succeeded = task.Started();
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

        // create a linked token that cancels the task when max 
        // task timeout is exheeded or if a scale in event occures
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        cts.CancelAfter(PwnInfraContext.Config.Worker.MaxTaskTimeout * 1000);

        Environment.SetEnvironmentVariable("PWNCTL_OUTPUT_PATH", $"{EnvironmentVariables.FS_MOUNT_POINT}/{task.Operation.Name.Value}/{task.Definition.Name.Value}");

        int exitCode = 0;
        StringBuilder stdout = null, stderr = new();
        try
        {
            PwnInfraContext.Logger.Information($"Running task #{task.Id}: " + task.Command);

            (exitCode, stdout, stderr) = await PwnInfraContext.CommandExecutor.ExecuteAsync(task.Command, stdin, token: cts.Token);

            task.Finished(exitCode, stderr.ToString());
        }
        catch (Exception ex)
        {
            if (ex is OperationCanceledException)
            {
                PwnInfraContext.Logger.Warning($"Task {task.Id} timed out.");
                task.Timedout(stderr.ToString());

                await PwnInfraContext.TaskQueueService.DequeueAsync(taskDTO);
            }
            else
            {
                PwnInfraContext.Logger.Exception(ex);
                task.Failed(stderr.ToString());
            }

            succeeded = await _taskRepo.TryUpdateAsync(task);
            if (!succeeded)
            {
                PwnInfraContext.Logger.Warning($"failed to update task records #{task.Id} state to FAILED.");
            }

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
        
        await PwnInfraContext.TaskQueueService.EnqueueBatchAsync(batches);
    }
}
