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
        Metadata["MessageGroupId"] = Guid.NewGuid().ToString();
    }

    public static List<OutputBatchDTO> FromLines(IEnumerable<string> lines, int taskId)
    {
        List<OutputBatchDTO> batches = new();

        OutputBatchDTO outputBatch = new(taskId);
        for (int max = 10, i = 0; i < lines.Count(); i++)
        {
            var line = lines.ElementAt(i);
            if ((string.Join(",", outputBatch.Lines).Length + line.Length) < 8000)
                outputBatch.Lines.Add(line);

            if (outputBatch.Lines.Count == max || (string.Join(",", outputBatch.Lines).Length + line.Length) >= 8000)
            {
                batches.Add(outputBatch);
                outputBatch = new(taskId);
                outputBatch.Lines.Add(line);
            }
        }
        batches.Add(outputBatch);

        return batches;
    }
}
