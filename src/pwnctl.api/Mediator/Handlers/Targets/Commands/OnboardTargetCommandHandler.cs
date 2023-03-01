using Microsoft.EntityFrameworkCore;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.dto.Targets.Commands;

using MediatR;

namespace pwnctl.api.Mediator.Handlers.Targets.Commands
{
    public sealed class OnboardTargetCommandHandler : IRequestHandler<OnboardTargetCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse> Handle(OnboardTargetCommand command, CancellationToken cancellationToken)
        {
            var existingProgram = await _context.Programs.FirstOrDefaultAsync(p => p.Name == command.Name);
            if (existingProgram != null)
                return MediatedResponse.Error("Target {0} already exists.", command.Name);

            var taskProfile = _context.TaskProfiles.FirstOrDefault(p => p.ShortName == command.TaskProfile.ShortName);
            if (taskProfile == null)
                return MediatedResponse.Error("Task Profile {0} not found.", command.TaskProfile.ShortName);

            command.TaskProfile = taskProfile;

            _context.Programs.Add(command);
            await _context.SaveChangesAsync();

            return MediatedResponse.Success();
        }
    }
}