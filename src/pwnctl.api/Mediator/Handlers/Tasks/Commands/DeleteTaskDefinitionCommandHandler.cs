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
    public sealed class DeleteTaskDefinitionCommandHandler : IRequestHandler<DeleteTaskDefinitionCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse> Handle(DeleteTaskDefinitionCommand command, CancellationToken cancellationToken)
        {
            var definition = _context.TaskDefinitions.FirstOrDefault(a => a.Name == ShortName.Create(command.Name));
            if (definition is null)
                return MediatedResponse.Error("Task Definition {0} not found.", command.Name);

            _context.Remove(definition);
            await _context.SaveChangesAsync(cancellationToken);

            return MediatedResponse.Success();
        }
    }
}
