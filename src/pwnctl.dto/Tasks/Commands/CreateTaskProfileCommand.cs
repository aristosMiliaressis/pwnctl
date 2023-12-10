namespace pwnctl.dto.Tasks.Commands;

using pwnctl.dto.Mediator;
using pwnctl.infra.Configuration;

public sealed class CreateTaskProfileCommand : TaskProfileRequestModel, MediatedRequest
{
    public static string Route => "/tasks/profiles";
    public static HttpMethod Verb => HttpMethod.Post;
}

public class TaskProfileRequestModel
{
    public string Profile { get; init; }
    public int Phase { get; init; }
    public List<TaskDefinitionDTO> TaskDefinitions { get; set; }
}
