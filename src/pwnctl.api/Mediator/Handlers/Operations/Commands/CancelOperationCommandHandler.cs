using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using MediatR;
using pwnctl.dto.Operations.Commands;
using pwnctl.app.Operations.Enums;
using pwnctl.infra.Scheduling;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.api.Mediator.Handlers.Operations.Commands
{
    public sealed class CancelOperationCommandHandler : IRequestHandler<CancelOperationCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new();
        private readonly EventBridgeClient _client = new();

        public async Task<MediatedResponse> Handle(CancelOperationCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
