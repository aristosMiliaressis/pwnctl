using System.Text.Json.Serialization;
using pwnctl.app.Tasks.Entities;

namespace pwnctl.app.Queueing.DTO;

public sealed class QueueTaskDTO
{
    public int TaskId { get; init; }
    public string Command { get; init; }

    [JsonIgnore]
    public Dictionary<string, string> Metadata { get; set; }

    public QueueTaskDTO(TaskEntry entry)
    {
        TaskId = entry.Id;
        Command = entry.WrappedCommand;
    }
}