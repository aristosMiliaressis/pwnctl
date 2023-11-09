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

    public async Task<bool> TryInitializeAsync(int opId)
    {
        try
        {
            var op = await _opRepo.FindAsync(opId);
            if (op is null)
            {
                PwnInfraContext.Logger.Warning($"Operation id #{opId} was not found.");
                return false;
            }

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
        catch (Exception ex)
        {
            PwnInfraContext.Logger.Warning($"Operation id #{opId} failed to initialize.");
            PwnInfraContext.Logger.Exception(ex);
            return false;
        }

        return true;
    }

    public async Task GenerateScheduledTasksAsync(Operation op, AssetRecord record, IEnumerable<TaskDefinition> taskDefinitions)
    {
        foreach (var defGroup in taskDefinitions.GroupBy(t => t.Filter))
        {
            if (!defGroup.First().Matches(record, minitoring: op.Type == OperationType.Monitor))
                continue;

            foreach (var def in defGroup)
            {
                if (def.Subject.Value != record.Asset.GetType().Name)
                    continue;

                var task = new TaskRecord(op, def, record);

                await PwnInfraContext.TaskRepository.AddAsync(task);

                await PwnInfraContext.TaskQueueService.EnqueueAsync<PendingTaskDTO>(new PendingTaskDTO(task));
            }
        }
    }
}
