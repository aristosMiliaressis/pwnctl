using Microsoft.EntityFrameworkCore;
using pwnctl.dto.Mediator;
using pwnwrk.infra.Persistence;
using pwnctl.dto.Targets.Commands;

using MediatR;

namespace pwnctl.api.MediatR.Handlers.Targets.Commands
{
    public sealed class CreateTargetCommandHandler : IRequestHandler<CreateTargetCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse> Handle(CreateTargetCommand command, CancellationToken cancellationToken)
        {
            var existingProgram = await _context.Programs.FirstOrDefaultAsync(p => p.Name == command.Name);
            if (existingProgram != null)
                return MediatedResponse.Error("Target %s already exists.", command.Name);

            _context.Programs.Add(command);
            await _context.SaveChangesAsync();

            return MediatedResponse.Success();
        }
    }
}