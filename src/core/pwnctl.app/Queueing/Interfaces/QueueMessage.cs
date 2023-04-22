using System.Text.Json.Serialization;

namespace pwnctl.app.Queueing.Interfaces;

public interface QueueMessage
{
    int TaskId { get; set; }
    
    [JsonIgnore]
    Dictionary<string, string> Metadata { get; set; }
}