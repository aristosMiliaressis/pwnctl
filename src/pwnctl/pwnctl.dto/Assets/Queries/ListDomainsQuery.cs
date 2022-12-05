namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.ViewModels;
using pwnctl.dto.Mediator;

public sealed class ListDomainsQuery : MediatedRequest<DomainListViewModel>
{
    public static string Route => "/assets/domains";
    public static HttpMethod Verb => HttpMethod.Get;
}