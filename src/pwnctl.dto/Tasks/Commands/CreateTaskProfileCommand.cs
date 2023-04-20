namespace pwnctl.dto.Tasks.Commands;

using pwnctl.dto.Mediator;

public sealed class CreateTaskProfileCommand : TaskProfileRequestModel, MediatedRequest
{
    public static string Route => "/tasks/profiles";
    public static HttpMethod Verb => HttpMethod.Post;
}

public class TaskProfileRequestModel
{
    public string ShortName { get; private init; }
    public List<TaskDefinitionRequestModel> TaskDefinitions { get; set; }
}