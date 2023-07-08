using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Tasks.Entities;

namespace pwnctl.app.Tasks.Interfaces;

public interface TaskRepository
{
    Task<List<TaskRecord>> ListAsync(int pageIdx, int pageSize = 4096);
    List<TaskDefinition> ListOutOfScope();

    Task<TaskRecord> FindAsync(int taskId);
    TaskRecord Find(AssetRecord asset, TaskDefinition definition);

    Task AddAsync(TaskRecord task);
    Task UpdateAsync(TaskRecord task);
}