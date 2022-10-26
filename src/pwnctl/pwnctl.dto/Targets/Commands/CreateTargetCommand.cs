namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;

using MediatR;

public class CreateTargetCommand : Program, IRequest<MediatorResult>
{

}