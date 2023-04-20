using pwnctl.dto.Scope.Queries;
using pwnctl.dto.Scope.Models;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using Microsoft.EntityFrameworkCore;
using pwnctl.dto.Scope.Commands;

namespace pwnctl.api.Mediator.Handlers.Scope.Commands
{
    public sealed class UpdateScopeAggregateCommandHandler : IRequestHandler<UpdateScopeAggregateCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public Task<MediatedResponse> Handle(UpdateScopeAggregateCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}