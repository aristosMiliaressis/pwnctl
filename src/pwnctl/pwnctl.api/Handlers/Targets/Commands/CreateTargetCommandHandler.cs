namespace pwnctl.api.Handlers.Targets.Commands;

using Microsoft.EntityFrameworkCore;
using pwnwrk.infra.MediatR;
using pwnwrk.infra.Persistence;
using pwnctl.dto.Targets.Commands;

using MediatR;

public class CreateTargetCommandHandler : IRequestHandler<CreateTargetCommand, MediatorResult>
{
    private readonly PwnctlDbContext _context = new PwnctlDbContext();

    public async Task<MediatorResult> Handle(CreateTargetCommand command, CancellationToken cancellationToken)
    {
        var existingProgram = await _context.Programs.FirstOrDefaultAsync(p => p.Name == command.Name);
        if (existingProgram != null)
            return MediatorResult.Error("Target %s already exists.", command.Name);

        _context.Programs.Add(command);
        await _context.SaveChangesAsync();

        return MediatorResult.Success();
    }
}