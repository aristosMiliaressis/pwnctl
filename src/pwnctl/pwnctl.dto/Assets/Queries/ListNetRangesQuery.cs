namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.ViewModels;
using pwnctl.dto.Mediator;

using MediatR;

public sealed class ListNetRangesQuery : IMediatedRequest<NetRangeListViewModel>
{
    public static string Route => "/assets/netranges";
    public static HttpMethod Verb => HttpMethod.Get;
}