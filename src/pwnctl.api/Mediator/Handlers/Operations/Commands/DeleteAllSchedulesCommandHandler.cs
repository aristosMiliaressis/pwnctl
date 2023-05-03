using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using MediatR;
using pwnctl.dto.Operations.Commands;
using pwnctl.app.Operations.Enums;
using pwnctl.infra.Scheduling;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.api.Mediator.Handlers.Operations.Commands
{
    public sealed class DeleteAllSchedulesCommandHandler : IRequestHandler<DeleteAllSchedulesCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new();
        private readonly EventBridgeScheduler _scheduler = new();

        public async Task<MediatedResponse> Handle(DeleteAllSchedulesCommand command, CancellationToken cancellationToken)
        {
            var ops = await _context.Operations.Where(a => a.Type != OperationType.Crawl).ToListAsync();

            foreach (var op in ops)
            {
                await _scheduler.DisableScheduledOperation(op);
            }

            return MediatedResponse.Success();
        }
    }
}
