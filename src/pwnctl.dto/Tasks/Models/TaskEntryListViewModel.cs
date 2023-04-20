namespace pwnctl.dto.Tasks.Models;

using pwnctl.app.Tasks.DTO;
using pwnctl.app.Tasks.Entities;

public sealed class TaskEntryListViewModel
{
    public IEnumerable<TaskEntryDTO> Tasks { get; init; }

    public TaskEntryListViewModel() { }

    public TaskEntryListViewModel(List<TaskEntry> tasks)
    {
        Tasks = tasks.Select(t => new TaskEntryDTO(t));
    }
}