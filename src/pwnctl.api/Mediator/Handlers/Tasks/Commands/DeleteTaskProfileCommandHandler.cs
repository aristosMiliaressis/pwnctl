using pwnctl.dto.Tasks.Queries;
using pwnctl.dto.Tasks.Models;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using Microsoft.EntityFrameworkCore;
using pwnctl.dto.Tasks.Commands;
using pwnctl.app.Common.ValueObjects;

namespace pwnctl.api.Mediator.Handlers.Tasks.Commands
{
    public sealed class DeleteTaskProfileCommandHandler : IRequestHandler<DeleteTaskProfileCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse> Handle(DeleteTaskProfileCommand command, CancellationToken cancellationToken)
        {
            var profile = _context.TaskProfiles.FirstOrDefault(a => a.Name == ShortName.Create(command.Name));
            if (profile is null)
                return MediatedResponse.Error("Task Profile {0} not found.", command.Name);

            _context.Remove(profile);
            await _context.SaveChangesAsync(cancellationToken);

            return MediatedResponse.Success();
        }
    }
}