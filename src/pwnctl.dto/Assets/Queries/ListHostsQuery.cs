namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.Models;
using pwnctl.dto.Mediator;

public sealed class ListHostsQuery : MediatedRequest<HostListViewModel>, PaginatedRequest
{
    public static string Route => "/assets/hosts";
    public static HttpMethod Verb => HttpMethod.Get;

    public int Page { get; set; }
}