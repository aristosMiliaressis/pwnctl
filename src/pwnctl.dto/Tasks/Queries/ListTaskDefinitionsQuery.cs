namespace pwnctl.dto.Tasks.Queries;

using pwnctl.dto.Tasks.Models;
using pwnctl.dto.Mediator;

public sealed class ListTaskDefinitionsQuery : MediatedRequest<TaskDefinitionListViewModel>
{
    public static string Route => "/tasks/definitions";
    public static HttpMethod Verb => HttpMethod.Get;
}