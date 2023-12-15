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
        private readonly EventBridgeClient _client = new();

        public async Task<MediatedResponse> Handle(DeleteAllSchedulesCommand command, CancellationToken cancellationToken)
        {
            var ops = await _context.Operations.ToListAsync();

            foreach (var op in ops)
            {
                if (op.Schedule is not null)
                    await _client.DisableSchedule(op);

                if (op.Type != OperationType.Monitor)
                    await _client.Unsubscribe(op);
            }

            return MediatedResponse.Success();
        }
    }
}
