namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.ViewModels;
using pwnctl.dto.Mediator;

public sealed class ListHostsQuery : IMediatedRequest<HostListViewModel>
{
    public static string Route => "/assets/hosts";
    public static HttpMethod Verb => HttpMethod.Get;
}