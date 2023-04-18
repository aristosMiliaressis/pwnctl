namespace pwnctl.dto.Targets.ViewModels;

using pwnctl.app.Tasks.DTO;
using pwnctl.app.Tasks.Entities;

public sealed class TasksViewModel
{
    public IEnumerable<TaskEntryDTO> Tasks { get; init; }

    public TasksViewModel() { }

    public TasksViewModel(List<TaskEntry> tasks)
    {
        Tasks = tasks.Select(t => new TaskEntryDTO(t));
    }
}