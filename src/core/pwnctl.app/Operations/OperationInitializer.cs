using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.Interfaces;
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
    private readonly AssetRepository _assetRepo;
    private readonly TaskRepository _taskRepo;
    private readonly TaskQueueService _taskQueueService;

    public OperationInitializer(OperationRepository opRepo, AssetRepository assetRepo,
                                    TaskRepository taskRepo, TaskQueueService taskQueueService)
    {
        _opRepo = opRepo;
        _assetRepo = assetRepo;
        _taskRepo = taskRepo;
        _taskQueueService = taskQueueService;
    }

    public async Task InitializeAsync(int opId)
    {
        var op = await _opRepo.FindAsync(opId);

        op.InitiatedAt = SystemTime.UtcNow();
        op.State = OperationState.Ongoing;
        await _opRepo.SaveAsync(op);

        var monitoringTasks = op.Policy.TaskProfile.TaskDefinitions.Where(def => def.MonitorRules.Schedule != null);

        int page = 0;
        while (true)
        {
            var records = await _assetRepo.ListInScopeAsync(op.ScopeId, monitoringTasks.Select(t => t.Subject).Distinct().ToArray(), page);

            foreach (var record in records)
            {
                await GenerateScheduledTasksAsync(op, record, monitoringTasks);
            }

            if (records.Count != 4096)
                break;

            page++;
        }

        await _opRepo.SaveAsync(op);
    }

    public async Task GenerateScheduledTasksAsync(Operation op, AssetRecord record, IEnumerable<TaskDefinition> monitoringTasks)
    {
        foreach (var def in monitoringTasks.Where(def => def.Matches(record, minitoring: op.Type == OperationType.Monitor)))
        {
            var task = new TaskRecord(op, def, record);

            await _taskRepo.AddAsync(task);

            await _taskQueueService.EnqueueAsync<PendingTaskDTO>(new PendingTaskDTO(task));
        }
    }
}
