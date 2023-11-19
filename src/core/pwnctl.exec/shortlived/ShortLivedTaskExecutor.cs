namespace pwnctl.exec.shortlived;

using pwnctl.app;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Tasks.Enums;
using pwnctl.infra.Repositories;
using pwnctl.infra.Persistence;
using pwnctl.infra;
using System.Text;

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
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            cts.Cancel();
        };

        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                await ExecutePendingTaskAsync(cts.Token);
            } 
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
            }
        }
    }

    private async Task ExecutePendingTaskAsync(CancellationToken stoppingToken)
    {
        ShortLivedTaskDTO taskDTO = null;
        try 
        {
            taskDTO = await PwnInfraContext.TaskQueueService.ReceiveAsync<ShortLivedTaskDTO>(stoppingToken);
            if (taskDTO is null)
            {
                PwnInfraContext.Logger.Information("task queue is empty, sleeping for 5 secs.");
                await StopAsync(stoppingToken);
                return;
            }
        } 
        catch (OperationCanceledException)
        {
            return;
        }

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

            (exitCode, stdout, stderr) = await PwnInfraContext.CommandExecutor.ExecuteAsync(task.Command, stdin);

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

        var lines = stdout.ToString()
                        .Split("\n")
                        .Where(l => !string.IsNullOrEmpty(l));

        PwnInfraContext.Logger.Information($"Task: {taskDTO.TaskId} produced {lines.Count()} lines");

        var batches = OutputBatchDTO.FromLines(lines, task.Id);
        
        await PwnInfraContext.TaskQueueService.EnqueueBatchAsync(batches);
    }
}
