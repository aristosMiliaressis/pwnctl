using Microsoft.EntityFrameworkCore;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.dto.Operations.Commands;

using MediatR;

namespace pwnctl.api.Mediator.Handlers.Operations.Commands
{
    public sealed class CreateOperationCommandHandler : IRequestHandler<CreateOperationCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse> Handle(CreateOperationCommand command, CancellationToken cancellationToken)
        {
            var existingProgram = await _context.Operations.FirstOrDefaultAsync(p => p.ShortName == command.ShortName);
            if (existingProgram != null)
                return MediatedResponse.Error("Target {0} already exists.", command.ShortName.Value);

            var scopeAggregate = _context.ScopeAggregates.FirstOrDefault(a => a.ShortName == command.Scope.ShortName);
            if (scopeAggregate == null)
                return MediatedResponse.Error("Scope Aggregate {0} not found.", command.Scope.ShortName.Value);

            command.Scope = scopeAggregate;

            var taskProfile = _context.TaskProfiles.FirstOrDefault(p => p.ShortName == command.Policy.TaskProfile.ShortName);
            if (taskProfile == null)
                return MediatedResponse.Error("Task Profile {0} not found.", command.Policy.TaskProfile.ShortName.Value);

            command.Policy.TaskProfile = taskProfile;

            _context.Operations.Add(command);
            await _context.SaveChangesAsync();

            return MediatedResponse.Success();
        }
    }
}