namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.ViewModels;
using pwnctl.dto.Mediator;

public sealed class ListNetRangesQuery : MediatedRequest<NetRangeListViewModel>
{
    public static string Route => "/assets/netranges";
    public static HttpMethod Verb => HttpMethod.Get;
}