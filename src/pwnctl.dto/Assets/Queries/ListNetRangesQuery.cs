namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.Models;
using pwnctl.dto.Mediator;

public sealed class ListNetRangesQuery : MediatedRequest<NetRangeListViewModel>, PaginatedRequest
{
    public static string Route => "/assets/netranges";
    public static HttpMethod Verb => HttpMethod.Get;

    public int Page { get; set; }
}