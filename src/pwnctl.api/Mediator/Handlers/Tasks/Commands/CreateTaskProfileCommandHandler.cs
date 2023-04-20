using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.dto.Tasks.Commands;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Common.ValueObjects;

namespace pwnctl.api.Mediator.Handlers.Tasks.Commands
{
    public sealed class CreateTaskProfileCommandHandler : IRequestHandler<CreateTaskProfileCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse> Handle(CreateTaskProfileCommand command, CancellationToken cancellationToken)
        {
            var profile = _context.TaskProfiles.FirstOrDefault(a => a.ShortName.Value == command.ShortName);
            if (profile != null)
                return MediatedResponse.Error("Task Profile {0} already exists.", command.ShortName);

            profile = new TaskProfile
            {
                ShortName = ShortName.Create(command.ShortName),
            };

            foreach (var def in command.TaskDefinitions)
            {
                var taskDef = _context.TaskDefinitions.FirstOrDefault(d => d.ShortName.Value == def.ShortName);
                if (taskDef == null)
                {
                    taskDef = new TaskDefinition
                    {
                        Name = def.ShortName,
                        Subject = def.Subject,
                        CommandTemplate = def.CommandTemplate,
                        Filter = def.Filter,
                        Aggressiveness = def.Aggressiveness,
                        IsActive = def.IsActive,
                        MatchOutOfScope = def.MatchOutOfScope
                    };

                    _context.Add(taskDef);
                }

                profile.TaskDefinitions.Add(taskDef);
            }

            _context.Add(profile);
            await _context.SaveChangesAsync(cancellationToken);

            return MediatedResponse.Success();
        }
    }
}