namespace pwnctl.api.Handlers.Targets.Commands;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;
using pwnctl.dto.Targets.Commands;

using MediatR;

public class AddScopeDefCommandHandler : IRequestHandler<AddScopeDefCommand, MediatorResult>
{
    public AddScopeDefCommandHandler()
    {

    }

    public Task<MediatorResult> Handle(AddScopeDefCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}