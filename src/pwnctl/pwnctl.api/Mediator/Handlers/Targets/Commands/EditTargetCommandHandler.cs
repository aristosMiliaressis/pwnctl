using pwnctl.dto;
using pwnctl.dto.Mediator;
using pwnctl.dto.Targets.Commands;

using MediatR;

namespace pwnctl.api.Mediator.Handlers.Targets.Commands
{
    public sealed class EditTargetCommandHandler : IRequestHandler<EditTargetCommand, MediatedResponse>
    {
        public EditTargetCommandHandler()
        {

        }

        public Task<MediatedResponse> Handle(EditTargetCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}