namespace pwnctl.api.Handlers.Targets.Commands;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;
using pwnwrk.infra.Persistence;
using pwnctl.dto.Targets.Commands;

using MediatR;

public class CreateTargetCommandHandler : IRequestHandler<CreateTargetCommand, MediatorResponse>
{
    private readonly PwnctlDbContext _context;

    public CreateTargetCommandHandler(PwnctlDbContext context)
    {
        _context = context;
    }

    public async Task<MediatorResponse> Handle(CreateTargetCommand command, CancellationToken cancellationToken)
    {
        var existingProgram = _context.Programs.FirstOrDefault(p => p.Name == command.Name);
        if (existingProgram != null)
            return MediatorResponse.Error("Target %s already exists.", command.Name);

        _context.Programs.Add(command);
        await _context.SaveChangesAsync();

        return MediatorResponse.Success();
    }
}