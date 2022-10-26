namespace pwnctl.api.Handlers.Targets.Commands;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;
using pwnctl.dto.Targets.Commands;

using MediatR;

public class EditScopeDefCommandHandler : IRequestHandler<EditScopeDefCommand, MediatorResult>
{
    public EditScopeDefCommandHandler()
    {

    }

    public Task<MediatorResult> Handle(EditScopeDefCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}