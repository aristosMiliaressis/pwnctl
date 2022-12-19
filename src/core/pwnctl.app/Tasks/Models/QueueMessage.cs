namespace pwnctl.app.Tasks.Models;

public interface MessageContent
{

}

public sealed class QueueMessage
{
    public MessageContent Content { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
}