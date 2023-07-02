using System.Text.Json.Serialization;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Tasks.Entities;

namespace pwnctl.app.Queueing.DTO;

public sealed class PendingTaskDTO : QueueMessage
{
    public int TaskId { get; set; }
    public string Command { get; init; }

    [JsonIgnore]
    public Dictionary<string, string> Metadata { get; set; } = new();

    public PendingTaskDTO() { }

    public PendingTaskDTO(TaskRecord record)
    {
        TaskId = record.Id;
        Command = record.Command;
        Metadata["MessageGroupId"] = record.Id.ToString();
    }
}
