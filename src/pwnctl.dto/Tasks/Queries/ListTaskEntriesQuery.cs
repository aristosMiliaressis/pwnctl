namespace pwnctl.dto.Tasks.Queries;

using pwnctl.dto.Tasks.Models;
using pwnctl.dto.Mediator;

public sealed class ListTaskEntriesQuery : MediatedRequest<TaskEntryListViewModel>
{
    public static string Route => "/tasks";
    public static HttpMethod Verb => HttpMethod.Get;
}