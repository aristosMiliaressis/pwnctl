using System.Text.Json.Serialization;

namespace pwnctl.app.Tasks.DTO;

public sealed class TaskDTO
{
    public int TaskId { get; init; }
    public string Command { get; init; }

    [JsonIgnore]
    public Dictionary<string, string> Metadata { get; set; }
}