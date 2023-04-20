namespace pwnctl.dto.Tasks.Commands;

using pwnctl.dto.Mediator;

public sealed class CreateTaskDefinitionCommand : TaskDefinitionRequestModel, MediatedRequest
{
    public static string Route => "/tasks/definitions";
    public static HttpMethod Verb => HttpMethod.Post;
}

public class TaskDefinitionRequestModel
{
    public string ShortName { get; private init; }
    public string CommandTemplate { get; set; }
    public bool IsActive { get; private init; }
    public int Aggressiveness { get; private init; }
    public string Subject { get; private set; }
    public string Filter { get; private init; }
    public bool MatchOutOfScope { get; private init; }
}