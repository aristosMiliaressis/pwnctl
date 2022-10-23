namespace pwnctl.dto.Targets.Queries;

using pwnctl.dto.Targets.ViewModels;
using pwnwrk.infra.MediatR;

using MediatR;

public class ListTargetsQuery : IRequest<MediatorResponse<TargetListViewModel>>
{

}