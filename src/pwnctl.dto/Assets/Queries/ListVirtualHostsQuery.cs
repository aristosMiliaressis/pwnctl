namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.Models;
using pwnctl.dto.Mediator;

public sealed class ListVirtualHostsQuery : MediatedRequest<VirtualHostListViewModel>, PaginatedRequest
{
    public static string Route => "/assets/vhosts";
    public static HttpMethod Verb => HttpMethod.Get;

    public int Page { get; set; }
}