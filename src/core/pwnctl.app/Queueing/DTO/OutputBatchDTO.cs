using System.Text.Json.Serialization;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Tasks.Entities;

namespace pwnctl.app.Queueing.DTO;

public sealed class OutputBatchDTO : QueueMessage
{
    public int TaskId { get; set; }
    public List<string> Lines { get; set; }

    [JsonIgnore]
    public Dictionary<string, string> Metadata { get; set; }

    public OutputBatchDTO() { }

    public OutputBatchDTO(TaskEntry entry, List<string> lines)
    {
        TaskId = entry.Id;
        Lines = lines;
    }
}