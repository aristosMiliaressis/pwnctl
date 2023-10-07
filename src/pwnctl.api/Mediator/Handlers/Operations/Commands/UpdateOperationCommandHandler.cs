using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.infra.Scheduling;

using MediatR;
using pwnctl.dto.Operations.Commands;
using Microsoft.EntityFrameworkCore;
using pwnctl.app.Common.ValueObjects;

namespace pwnctl.api.Mediator.Handlers.Operations.Commands
{
    public sealed class UpdateOperationCommandHandler : IRequestHandler<UpdateOperationCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();
        private readonly EventBridgeScheduler _scheduler = new();

        public async Task<MediatedResponse> Handle(UpdateOperationCommand command, CancellationToken cancellationToken)
        {
            var existingOperation = await _context.Operations.FirstOrDefaultAsync(p => p.Name == ShortName.Create(command.Name));
            if (existingOperation is null)
                return MediatedResponse.Error("Operation {0} not found.", command.Name);

            existingOperation.State = command.State;

            if (existingOperation.Schedule is not null)
                await _scheduler.DisableScheduledOperation(existingOperation);

            await _context.SaveChangesAsync(cancellationToken);

            return MediatedResponse.Success();
        }
    }
}