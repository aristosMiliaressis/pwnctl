namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.Models;
using pwnctl.dto.Mediator;

public sealed class ListNetworkHostsQuery : MediatedRequest<NetworkHostListViewModel>, PaginatedRequest
{
    public static string Route => "/assets/hosts";
    public static HttpMethod Verb => HttpMethod.Get;

    public int Page { get; set; }
}