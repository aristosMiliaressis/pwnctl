namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.Models;
using pwnctl.dto.Mediator;

public sealed class ListServicesQuery : MediatedRequest<ServiceListViewModel>
{
    public static string Route => "/assets/services";
    public static HttpMethod Verb => HttpMethod.Get;
}