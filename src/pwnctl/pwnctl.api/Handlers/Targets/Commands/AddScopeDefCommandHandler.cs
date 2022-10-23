namespace pwnctl.api.Handlers.Targets.Commands;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;
using pwnctl.dto.Targets.Commands;

using MediatR;

public class AddScopeDefCommandHandler : IRequestHandler<AddScopeDefCommand, MediatorResponse>
{
    public AddScopeDefCommandHandler()
    {

    }

    public Task<MediatorResponse> Handle(AddScopeDefCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}