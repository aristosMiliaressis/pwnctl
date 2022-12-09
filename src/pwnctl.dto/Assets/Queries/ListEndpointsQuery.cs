namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.ViewModels;
using pwnctl.dto.Mediator;

public sealed class ListEndpointsQuery : MediatedRequest<EndpointListViewModel>
{
    public static string Route => "/assets/endpoints";
    public static HttpMethod Verb => HttpMethod.Get;
}