namespace pwnctl.dto.Tasks.Models;

using pwnctl.app.Tasks.DTO;
using pwnctl.app.Tasks.Entities;
using pwnctl.dto.Mediator;

public sealed class TaskEntryListViewModel : PaginatedViewModel<TaskEntryDTO>
{
    public TaskEntryListViewModel() { }

    public TaskEntryListViewModel(List<TaskEntry> tasks)
    {
        Rows = tasks.Select(t => new TaskEntryDTO(t)).ToList();
    }
}