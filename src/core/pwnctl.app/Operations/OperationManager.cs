using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Common;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.app.Operations.Interfaces;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.kernel;

namespace pwnctl.app.Operations;

public class OperationManager
{
    private readonly OperationRepository _opRepo;
    private readonly OperationStateSubscriptionService _subscriptionService;

    public OperationManager(OperationRepository opRepo, OperationStateSubscriptionService subscriptionService)
    {
        _opRepo = opRepo;
        _subscriptionService = subscriptionService;
    }

    public async Task<bool> TryHandleAsync(int opId)
    {
        try
        {
            var op = await _opRepo.FindAsync(opId);
            if (op is null)
            {
                PwnInfraContext.Logger.Warning($"Operation id #{opId} was not found.");
                return false;
            }

            if (op.State == OperationState.Pending)
            {
                await InitializeAsync(op);
                return true;
            }

            await TerminateAsync(op);
        }
        catch (Exception ex)
        {
            PwnInfraContext.Logger.Warning($"Operation id #{opId} failed to initialize.");
            PwnInfraContext.Logger.Exception(ex);
            return false;
        }

        return true;    
    }

    public async Task InitializeAsync(Operation op)
    {
        op.Initialize();
        await _opRepo.SaveAsync(op);

        var taskDefinitions = op.Policy.TaskProfiles
                                    .SelectMany(p => p.TaskProfile.TaskDefinitions);

        if (op.Type == OperationType.Monitor)
            taskDefinitions = taskDefinitions.Where(def => def.MonitorRules.Schedule is not null);

        int page = 0;
        while (true)
        {
            var records = await PwnInfraContext.AssetRepository.ListInScopeAsync(op.ScopeId, taskDefinitions.Select(t => t.Subject).Distinct().ToArray(), page);

            foreach (var record in records)
            {
                await generateScheduledTasksAsync(op, record, taskDefinitions);
            }

            if (records.Count != PwnInfraContext.Config.Api.BatchSize)
                break;

            page++;
        }

        await _opRepo.SaveAsync(op);

        if (op.Type == OperationType.Scan)
        {
            await _subscriptionService.Subscribe(op);
        }
    }

    public async Task TerminateAsync(Operation op)
    {
        if (op.Type == OperationType.Scan || op.Type == OperationType.Crawl)
        {
            await _subscriptionService.Unsubscribe(op);
        }

        op.Terminate();
        await _opRepo.SaveAsync(op);
    }

    private async Task generateScheduledTasksAsync(Operation op, AssetRecord record, IEnumerable<TaskDefinition> taskDefinitions)
    {
        List<LongLivedTaskDTO> longLivedTasks = new();
        List<ShortLivedTaskDTO> shortLivedTasks = new();
        foreach (var def in taskDefinitions.Where(def => def.Matches(record, minitoring: op.Type == OperationType.Monitor)))
        {
            var task = new TaskRecord(op, def, record);

            await PwnInfraContext.TaskRepository.AddAsync(task);

            if (def.ShortLived) shortLivedTasks.Add(new ShortLivedTaskDTO(task));
            else longLivedTasks.Add(new LongLivedTaskDTO(task));
        }

        await PwnInfraContext.TaskQueueService.EnqueueBatchAsync(longLivedTasks);
        await PwnInfraContext.TaskQueueService.EnqueueBatchAsync(shortLivedTasks);
    }
}
