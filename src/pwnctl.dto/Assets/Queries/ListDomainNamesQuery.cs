namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.Models;
using pwnctl.dto.Mediator;

public sealed class ListDomainNamesQuery : MediatedRequest<DomainNameListViewModel>, PaginatedRequest
{
    public static string Route => "/assets/domains";
    public static HttpMethod Verb => HttpMethod.Get;

    public int Page { get; set; }
}