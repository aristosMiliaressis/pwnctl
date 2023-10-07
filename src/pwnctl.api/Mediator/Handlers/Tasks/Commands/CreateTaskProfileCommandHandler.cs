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
            var profile = _context.TaskProfiles.FirstOrDefault(a => a.Name == ShortName.Create(command.Profile));
            if (profile is not null)
                return MediatedResponse.Error("Task Profile {0} already exists.", command.Profile);

            profile = new TaskProfile(command.Profile, new());

            foreach (var def in command.TaskDefinitions)
            {
                var taskDef = _context.TaskDefinitions.FirstOrDefault(d => d.Name == ShortName.Create(def.Name));
                if (taskDef is null)
                {
                    taskDef = def.ToEntity();

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
