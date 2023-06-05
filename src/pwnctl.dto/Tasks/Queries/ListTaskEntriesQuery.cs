namespace pwnctl.dto.Tasks.Queries;

using pwnctl.dto.Tasks.Models;
using pwnctl.dto.Mediator;

public sealed class ListTaskEntriesQuery : MediatedRequest<TaskEntryListViewModel>, PaginatedRequest
{
    public static string Route => "/tasks";
    public static HttpMethod Verb => HttpMethod.Get;

    public int Page { get; set; }
}