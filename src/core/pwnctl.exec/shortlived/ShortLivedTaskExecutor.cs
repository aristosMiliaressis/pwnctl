namespace pwnctl.exec.shortlived;

using pwnctl.app;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Tasks.Enums;
using pwnctl.app.Common.Extensions;
using pwnctl.infra.Repositories;
using pwnctl.infra.Persistence;
using pwnctl.infra;
using System.Text;
using pwnctl.infra.Configuration;

public sealed class ShortLivedTaskExecutor : LifetimeService
{
    private static readonly QueryRunner _queryRunner = new();
    private static readonly TaskDbRepository _taskRepo = new();

    public ShortLivedTaskExecutor(IHostApplicationLifetime svcLifetime)
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
            }
        }
    }

    private async Task ExecutePendingTaskAsync(CancellationToken stoppingToken)
    {
        var taskDTO = await PwnInfraContext.TaskQueueService.ReceiveAsync<ShortLivedTaskDTO>(stoppingToken);
        if (taskDTO is null)
        {
            PwnInfraContext.Logger.Information("task queue is empty, exiting.");
            return;
        }

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

        succeeded = await _taskRepo.TryUpdateAsync(task, stoppingToken);
        if (!succeeded)
        {
            PwnInfraContext.Logger.Warning($"failed to update task records #{task.Id} state to STARTED.");
        }

        // create a linked token that cancels the task when max 
        // task timeout is exheeded or if a scale in event occures
        CancellationTokenSource cts = new CancellationTokenSource();
        cts.CancelAfter((PwnInfraContext.Config.Worker.MaxTaskTimeout - 30) * 1000);

        Environment.SetEnvironmentVariable("PWNCTL_OUTPUT_PATH", $"{EnvironmentVariables.FS_MOUNT_POINT}/{task.Operation.Name.Value}/{task.Definition.Name.Value}");

        int exitCode = 0;
        StringBuilder stdout = null, stderr = new();
        try
        {
            PwnInfraContext.Logger.Information($"Running task #{task.Id}: {task.Command}");

            (exitCode, stdout, stderr) = await PwnInfraContext.CommandExecutor.ExecuteAsync(task.Command, stdin, cts.Token);

            task.Finished(exitCode, stderr.ToString());
        }
        catch (Exception ex)
        {
            if (ex is OperationCanceledException)
            {
                PwnInfraContext.Logger.Warning($"Task {task.Id} timed out.");
                task.Timedout(stderr.ToString());

                // return the task to the queue, if this occures to many times,
                // the task will be put in the dead letter queue
                await PwnInfraContext.TaskQueueService.ChangeMessageVisibilityAsync(taskDTO, 0);
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

        var lines = stdout.ToString()
                        .Split("\n")
                        .Where(l => !string.IsNullOrEmpty(l));

        PwnInfraContext.Logger.Information($"Task: {taskDTO.TaskId} produced {lines.Count()} lines");

        var batches = OutputBatchDTO.FromLines(lines, task.Id);
        
        await PwnInfraContext.TaskQueueService.EnqueueBatchAsync(batches);
    }
}
