namespace pwnctl.dto.Targets.Queries;

using pwnctl.dto.Targets.ViewModels;
using pwnctl.dto.Mediator;

using MediatR;

public sealed class ListTargetsQuery : IMediatedRequest<TargetListViewModel>
{
    public static string Route => "/targets";
    public static HttpMethod Method => HttpMethod.Get;
}