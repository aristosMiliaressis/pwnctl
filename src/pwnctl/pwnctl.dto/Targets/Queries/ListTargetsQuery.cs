namespace pwnctl.dto.Targets.Queries;

using pwnctl.dto.Targets.ViewModels;
using pwnwrk.infra.MediatR;

using MediatR;

public class ListTargetsQuery : IApiRequest<TargetListViewModel>, IRequest<MediatorResult<TargetListViewModel>>
{
    public static string Route => "/targets";
    public static HttpMethod Method => HttpMethod.Get;
}