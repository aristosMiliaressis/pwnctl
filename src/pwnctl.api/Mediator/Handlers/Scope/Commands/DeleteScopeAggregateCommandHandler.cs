using pwnctl.dto.Mediator;
using pwnctl.dto.Scope.Commands;
using pwnctl.infra.Persistence;
using MediatR;
using pwnctl.app.Common.ValueObjects;

namespace pwnctl.api.Mediator.Handlers.Scope.Commands
{
    public sealed class DeleteScopeAggregateCommandHandler : IRequestHandler<DeleteScopeAggregateCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse> Handle(DeleteScopeAggregateCommand command, CancellationToken cancellationToken)
        {
            var scopeAggregate = _context.ScopeAggregates.FirstOrDefault(a => a.Name == ShortName.Create(command.Name));
            if (scopeAggregate is null)
                return MediatedResponse.Error("Scope Aggregate {0} not found.", command.Name);

            _context.Remove(scopeAggregate);
            await _context.SaveChangesAsync(cancellationToken);

            return MediatedResponse.Success();
        }
    }
}