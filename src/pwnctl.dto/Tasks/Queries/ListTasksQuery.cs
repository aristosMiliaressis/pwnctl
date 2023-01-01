namespace pwnctl.dto.Targets.Queries;

using pwnctl.dto.Targets.ViewModels;
using pwnctl.dto.Mediator;

public sealed class ListTasksQuery : MediatedRequest<TasksViewModel>
{
    public static string Route => "/tasks";
    public static HttpMethod Verb => HttpMethod.Get;
}