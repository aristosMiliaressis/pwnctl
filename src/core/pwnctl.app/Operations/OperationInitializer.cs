using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Interfaces;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Tasks.Interfaces;

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

        op.InitiatedAt = DateTime.UtcNow;

        var records = await _assetRepo.ListInScopeAsync(op.ScopeId);

        foreach (var record in records)
        {
            await GenerateScheduledTasksAsync(op, record);
        }

        await _opRepo.SaveAsync(op);
    }

    public async Task GenerateScheduledTasksAsync(Operation op, AssetRecord record)
    {
        foreach (var def in op.Policy.TaskProfile.TaskDefinitions.Where(def => def.Matches(record)))
        {
            var task = new TaskEntry(op, def, record);
            record.Tasks.Add(task);

            await _taskRepo.AddAsync(task);
            await _taskQueueService.EnqueueAsync<PendingTaskDTO>(new PendingTaskDTO(task));
        }
    }
}
