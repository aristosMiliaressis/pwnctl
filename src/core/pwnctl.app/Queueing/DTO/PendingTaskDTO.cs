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

    public PendingTaskDTO(TaskEntry entry)
    {
        TaskId = entry.Id;
        Command = entry.Command;
        Metadata["MessageGroupId"] = entry.Id.ToString();
    }
}
