namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.Models;
using pwnctl.dto.Mediator;

public sealed class ListEndpointsQuery : MediatedRequest<EndpointListViewModel>, PaginatedRequest
{
    public static string Route => "/assets/endpoints";
    public static HttpMethod Verb => HttpMethod.Get;
    
    public int Page { get; set; }
}