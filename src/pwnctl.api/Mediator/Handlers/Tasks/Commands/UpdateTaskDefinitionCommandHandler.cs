using pwnctl.dto.Tasks.Queries;
using pwnctl.dto.Tasks.Models;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using Microsoft.EntityFrameworkCore;
using pwnctl.dto.Tasks.Commands;

namespace pwnctl.api.Mediator.Handlers.Tasks.Commands
{
    public sealed class UpdateTaskDefinitionCommandHandler : IRequestHandler<UpdateTaskDefinitionCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public Task<MediatedResponse> Handle(UpdateTaskDefinitionCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}