using pwnctl.dto;
using pwnctl.dto.Mediator;
using pwnctl.dto.Targets.Commands;

using MediatR;

namespace pwnctl.api.MediatR.Handlers.Targets.Commands
{
    public class EditTargetCommandHandler : IRequestHandler<EditTargetCommand, MediatedResponse>
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