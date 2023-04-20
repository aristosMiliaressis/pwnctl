using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.dto.Operations.Commands;

namespace pwnctl.api.Mediator.Handlers.Operations.Commands
{
    public sealed class UpdateOperationCommandHandler : IRequestHandler<UpdateOperationCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public Task<MediatedResponse> Handle(UpdateOperationCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}