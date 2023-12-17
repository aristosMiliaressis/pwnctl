namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.Models;
using pwnctl.dto.Mediator;

public sealed class ListHttpEndpointsQuery : MediatedRequest<HttpEndpointListViewModel>, PaginatedRequest
{
    public static string Route => "/assets/endpoints";
    public static HttpMethod Verb => HttpMethod.Get;
    
    public int Page { get; set; }
}