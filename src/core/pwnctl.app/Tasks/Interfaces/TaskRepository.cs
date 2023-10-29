using pwnctl.app.Assets.Entities;
using pwnctl.app.Tasks.Entities;

namespace pwnctl.app.Tasks.Interfaces;

public interface TaskRepository
{
    Task<IEnumerable<TaskRecord>> ListAsync(int pageIdx);

    Task<TaskRecord> FindAsync(int taskId);
    TaskRecord Find(AssetRecord asset, TaskDefinition definition);

    Task AddAsync(TaskRecord task);
    Task UpdateAsync(TaskRecord task);
}