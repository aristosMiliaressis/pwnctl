namespace pwnctl.app.Tasks.Models;

public sealed class TaskDTO : MessageContent
{
    public int TaskId { get; init; }
    public string Command { get; init; }
}