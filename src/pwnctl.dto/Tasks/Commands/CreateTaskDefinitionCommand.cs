namespace pwnctl.dto.Tasks.Commands;

using pwnctl.dto.Mediator;
using pwnctl.infra.Configuration;

public sealed class CreateTaskDefinitionCommand : TaskDefinitionDTO, MediatedRequest
{
    public static string Route => "/tasks/definitions";
    public static HttpMethod Verb => HttpMethod.Post;
}
