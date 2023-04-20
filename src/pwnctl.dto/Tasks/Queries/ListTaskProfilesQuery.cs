namespace pwnctl.dto.Tasks.Queries;

using pwnctl.dto.Tasks.Models;
using pwnctl.dto.Mediator;

public sealed class ListTaskProfilesQuery : MediatedRequest<TaskProfileListViewModel>
{
    public static string Route => "/tasks/profiles";
    public static HttpMethod Verb => HttpMethod.Get;
}