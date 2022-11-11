namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.ViewModels;
using pwnctl.dto.Mediator;

public sealed class ListServicesQuery : IMediatedRequest<ServiceListViewModel>
{
    public static string Route => "/assets/services";
    public static HttpMethod Verb => HttpMethod.Get;
}