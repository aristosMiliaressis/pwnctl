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

public class OperationInitializer
{
    private readonly OperationRepository _opRepo;

    public OperationInitializer(OperationRepository opRepo)
    {
        _opRepo = opRepo;
    }

    public async Task InitializeAsync(int opId)
    {
        var op = await _opRepo.FindAsync(opId);

        op.InitiatedAt = SystemTime.UtcNow();
        op.State = OperationState.Ongoing;
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
                await GenerateScheduledTasksAsync(op, record, taskDefinitions);
            }

            if (records.Count != PwnInfraContext.Config.Api.BatchSize)
                break;

            page++;
        }

        await _opRepo.SaveAsync(op);
    }

    public async Task GenerateScheduledTasksAsync(Operation op, AssetRecord record, IEnumerable<TaskDefinition> taskDefinitions)
    {
        foreach (var def in taskDefinitions.Where(def => def.Matches(record, minitoring: op.Type == OperationType.Monitor)))
        {
            var task = new TaskRecord(op, def, record);

            await PwnInfraContext.TaskRepository.AddAsync(task);

            await PwnInfraContext.TaskQueueService.EnqueueAsync<PendingTaskDTO>(new PendingTaskDTO(task));
        }
    }
}
