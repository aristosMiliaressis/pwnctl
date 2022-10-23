namespace pwnctl.api.Handlers.Targets.Commands;

using pwnctl.dto;
using pwnwrk.infra.MediatR;
using pwnctl.dto.Targets.Commands;

using MediatR;

public class EditTargetCommandHandler : IRequestHandler<EditTargetCommand, MediatorResponse>
{
    public EditTargetCommandHandler()
    {

    }

    public Task<MediatorResponse> Handle(EditTargetCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}