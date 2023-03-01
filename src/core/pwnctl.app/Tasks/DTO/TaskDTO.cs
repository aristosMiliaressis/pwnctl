using pwnctl.app.Tasks.Entities;

namespace pwnctl.app.Tasks.DTO;

public sealed class TaskDTO
{
    public int Id { get; set; }
    public string ShortName { get; set; }
    public string Subject { get; set; }
    public string Asset { get; set; }
    public string State { get; set; }
    public DateTime QueuedAt { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public double Duration { get; set; }
    public int? ExitCode { get; set; }

    public TaskDTO() { }

    public TaskDTO(TaskEntry entry)
    {
        Id = entry.Id;
        ShortName = entry.Definition.ShortName;
        Subject = entry.Definition.SubjectClass.Class;
        Asset = entry.Record.Asset.ToString();
        State = entry.State.ToString();
        QueuedAt = entry.QueuedAt;
        StartedAt = entry.StartedAt;
        FinishedAt = entry.FinishedAt;
        ExitCode = entry.ExitCode;
        Duration = (entry.FinishedAt - entry.StartedAt).TotalSeconds;
    }
}