namespace pwnctl.dto.Tasks.Queries;

using pwnctl.dto.Tasks.Models;
using pwnctl.dto.Mediator;

public sealed class ListTaskRecordsQuery : MediatedRequest<TaskRecordListViewModel>, PaginatedRequest
{
    public static string Route => "/tasks";
    public static HttpMethod Verb => HttpMethod.Get;

    public int Page { get; set; }
}