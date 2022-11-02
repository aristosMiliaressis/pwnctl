using pwnctl.dto.Mediator;
using pwnctl.dto.Targets.Commands;

using MediatR;

namespace pwnctl.api.MediatR.Handlers.Targets.Commands
{
    public class AddScopeDefCommandHandler : IRequestHandler<AddScopeDefCommand, MediatedResponse>
    {
        public AddScopeDefCommandHandler()
        {

        }

        public Task<MediatedResponse> Handle(AddScopeDefCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}