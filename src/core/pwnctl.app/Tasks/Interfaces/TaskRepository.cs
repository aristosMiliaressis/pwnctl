using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Tasks.Entities;

namespace pwnctl.app.Tasks.Interfaces;

public interface TaskRepository
{
    Task<List<TaskEntry>> ListEntriesAsync(int pageIdx, int pageSize = 4096);

    Task<TaskEntry> FindAsync(int taskId);
    TaskEntry Find(AssetRecord asset, TaskDefinition definition);

    Task AddAsync(TaskEntry task);
    Task UpdateAsync(TaskEntry task);
}