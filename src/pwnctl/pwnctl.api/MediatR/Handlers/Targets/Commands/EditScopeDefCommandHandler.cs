using pwnctl.dto.Mediator;
using pwnctl.dto.Targets.Commands;

using MediatR;

namespace pwnctl.api.MediatR.Handlers.Targets.Commands
{
    public sealed class EditScopeDefCommandHandler : IRequestHandler<EditScopeDefCommand, MediatedResponse>
    {
        public EditScopeDefCommandHandler()
        {

        }

        public Task<MediatedResponse> Handle(EditScopeDefCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}