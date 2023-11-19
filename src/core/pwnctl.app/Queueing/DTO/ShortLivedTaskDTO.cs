using System.Text.Json.Serialization;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Tasks.Entities;

namespace pwnctl.app.Queueing.DTO;

public sealed class ShortLivedTaskDTO : QueueMessage
{
    public int TaskId { get; set; }
    public string Command { get; set; }

    [JsonIgnore]
    public Dictionary<string, string> Metadata { get; set; } = new();

    public ShortLivedTaskDTO() { }

    public ShortLivedTaskDTO(TaskRecord record)
    {
        TaskId = record.Id;
        Command = record.Command;
        Metadata["MessageGroupId"] = record.Id.ToString();
        Metadata["MessageDeduplicationId"] = record.Id.ToString();
    }
}
