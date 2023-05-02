using System.Text.Json.Serialization;
using pwnctl.app.Queueing.Interfaces;

namespace pwnctl.app.Queueing.DTO;

public sealed class OutputBatchDTO : QueueMessage
{
    public int TaskId { get; set; }
    public List<string> Lines { get; set; } = new List<string>();

    [JsonIgnore]
    public Dictionary<string, string> Metadata { get; set; } = new();

    public OutputBatchDTO() { }

    public OutputBatchDTO(int taskId)
    {
        TaskId = taskId;
    }
}
