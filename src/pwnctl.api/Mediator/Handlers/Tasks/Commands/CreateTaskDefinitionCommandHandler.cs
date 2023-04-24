using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.dto.Tasks.Commands;
using pwnctl.app.Common.ValueObjects;

namespace pwnctl.api.Mediator.Handlers.Tasks.Commands
{
    public sealed class CreateTaskDefinitionCommandHandler : IRequestHandler<CreateTaskDefinitionCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse> Handle(CreateTaskDefinitionCommand command, CancellationToken cancellationToken)
        {
            var definition = _context.TaskDefinitions.FirstOrDefault(a => a.ShortName == ShortName.Create(command.ShortName));
            if (definition != null)
                return MediatedResponse.Error("Task Definition {0} already exists.", command.ShortName);

            definition = new app.Tasks.Entities.TaskDefinition
            {
                Name = command.ShortName,
                Subject = command.Subject,
                CommandTemplate = command.CommandTemplate,
                Filter = command.Filter,
                Aggressiveness = command.Aggressiveness,
                IsActive = command.IsActive,
                MatchOutOfScope = command.MatchOutOfScope
            };

            _context.Add(definition);
            await _context.SaveChangesAsync(cancellationToken);

            return MediatedResponse.Success();
        }
    }
}