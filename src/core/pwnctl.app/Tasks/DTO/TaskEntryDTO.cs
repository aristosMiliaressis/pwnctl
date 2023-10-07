using pwnctl.app.Tasks.Entities;

namespace pwnctl.app.Tasks.DTO;

public sealed class TaskRecordDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Subject { get; set; }
    public string Asset { get; set; }
    public string State { get; set; }
    public DateTime QueuedAt { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public double Duration { get; set; }
    public int? ExitCode { get; set; }
    public int RunCount { get; set; }
    public string? StdErr { get; set; }

    public TaskRecordDTO() { }

    public TaskRecordDTO(TaskRecord record)
    {
        Id = record.Id;
        Name = record.Definition.Name.Value;
        Subject = record.Definition.Subject.Value;
        Asset = record.Record.Asset.ToString();
        State = record.State.ToString();
        QueuedAt = record.QueuedAt;
        StartedAt = record.StartedAt;
        FinishedAt = record.FinishedAt;
        ExitCode = record.ExitCode;
        RunCount = record.RunCount;
        StdErr = record.Stderr;
        Duration = (record.FinishedAt - record.StartedAt).TotalSeconds;
    }
}
