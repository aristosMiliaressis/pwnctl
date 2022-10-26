namespace pwnctl.api.Handlers.Targets.Commands;

using pwnctl.dto;
using pwnwrk.infra.MediatR;
using pwnctl.dto.Targets.Commands;

using MediatR;

public class EditTargetCommandHandler : IRequestHandler<EditTargetCommand, MediatorResult>
{
    public EditTargetCommandHandler()
    {

    }

    public Task<MediatorResult> Handle(EditTargetCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}