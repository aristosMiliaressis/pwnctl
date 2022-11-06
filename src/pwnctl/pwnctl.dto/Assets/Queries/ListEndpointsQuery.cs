namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.ViewModels;
using pwnctl.dto.Mediator;

using MediatR;

public sealed class ListEndpointsQuery : IMediatedRequest<EndpointListViewModel>
{
    public static string Route => "/assets/endpoints";
    public static HttpMethod Verb => HttpMethod.Get;
}