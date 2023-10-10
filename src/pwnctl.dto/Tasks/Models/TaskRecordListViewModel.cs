namespace pwnctl.dto.Tasks.Models;

using pwnctl.app.Tasks.DTO;
using pwnctl.app.Tasks.Entities;
using pwnctl.dto.Mediator;

public sealed class TaskRecordListViewModel : PaginatedViewModel<TaskRecordDTO>
{
    public TaskRecordListViewModel() { }

    public TaskRecordListViewModel(IEnumerable<TaskRecord> tasks)
    {
        Rows = tasks.Select(t => new TaskRecordDTO(t)).ToList();
    }
}