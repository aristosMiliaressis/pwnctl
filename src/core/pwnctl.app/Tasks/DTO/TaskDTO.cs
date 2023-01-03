using pwnctl.app.Tasks.Entities;

namespace pwnctl.app.Tasks.DTO;

public sealed class TaskDTO
{
    public string ShortName { get; set; }
    public string Subject { get; set; }
    public string State { get; set; }
    public string Asset { get; set; }
    public DateTime QueuedAt { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public int? ExitCode { get; set; }

    public TaskDTO() { }

    public TaskDTO(TaskEntry entry)
    {
        ShortName = entry.Definition.ShortName;
        Subject = entry.Definition.SubjectClass.Class;
        State = entry.State.ToString();
        Asset = entry.Record.Asset.ToString();
        QueuedAt = entry.QueuedAt;
        StartedAt = entry.StartedAt;
        FinishedAt = entry.FinishedAt;
        ExitCode = entry.ExitCode;
    }
}