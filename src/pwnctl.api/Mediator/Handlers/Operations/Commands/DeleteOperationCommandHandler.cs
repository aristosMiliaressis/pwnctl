using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using MediatR;
using pwnctl.dto.Operations.Commands;
using pwnctl.app.Common.ValueObjects;

namespace pwnctl.api.Mediator.Handlers.Operations.Commands
{
    public sealed class DeleteOperationCommandHandler : IRequestHandler<DeleteOperationCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse> Handle(DeleteOperationCommand command, CancellationToken cancellationToken)
        {
            var op = _context.Operations.FirstOrDefault(a => a.ShortName == ShortName.Create(command.ShortName));
            if (op == null)
                return MediatedResponse.Error("Operation {0} not found.", command.ShortName);

            _context.Remove(op);
            await _context.SaveChangesAsync(cancellationToken);

            return MediatedResponse.Success();
        }
    }
}