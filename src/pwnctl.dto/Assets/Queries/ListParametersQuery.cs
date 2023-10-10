namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.Models;
using pwnctl.dto.Mediator;

public sealed class ListParametersQuery : MediatedRequest<HttpParameterListViewModel>, PaginatedRequest
{
    public static string Route => "/assets/params/";
    public static HttpMethod Verb => HttpMethod.Get;
    public static bool IsPaginated => true;

    public int Page { get; set; }
}