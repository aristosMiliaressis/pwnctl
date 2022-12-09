namespace pwnctl.dto.Targets.Queries;

using pwnctl.dto.Targets.ViewModels;
using pwnctl.dto.Mediator;

public sealed class ListTargetsQuery : MediatedRequest<TargetListViewModel>
{
    public static string Route => "/targets";
    public static HttpMethod Verb => HttpMethod.Get;
}