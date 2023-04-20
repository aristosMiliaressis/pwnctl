using pwnctl.app.Scope.Entities;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.dto.Scope.Commands;
using pwnctl.app.Common.ValueObjects;

namespace pwnctl.api.Mediator.Handlers.Scope.Commands
{
    public sealed class CreateScopeAggregateCommandHandler : IRequestHandler<CreateScopeAggregateCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse> Handle(CreateScopeAggregateCommand command, CancellationToken cancellationToken)
        {
            var scopeAggregate = _context.ScopeAggregates.FirstOrDefault(a => a.ShortName.Value == command.ShortName);
            if (scopeAggregate != null)
                return MediatedResponse.Error("Scope Aggregate {0} already exists.", command.ShortName);

            scopeAggregate = new app.Scope.Entities.ScopeAggregate
            {
                ShortName = ShortName.Create(command.ShortName),
                Description = command.Description
            };

            foreach (var def in command.ScopeDefinitions)
            {
                var scopeDef = _context.ScopeDefinitions.FirstOrDefault(d => d.Pattern == def.Pattern && d.Type == def.Type);
                if (scopeDef == null)
                {
                    scopeDef = new ScopeDefinition
                    {
                        Type = def.Type,
                        Pattern = def.Pattern
                    };

                    _context.ScopeDefinitions.Add(scopeDef);
                }

                scopeAggregate.Definitions.Add(new ScopeDefinitionAggregate(scopeAggregate, scopeDef));
            }

            _context.Add(scopeAggregate);
            await _context.SaveChangesAsync(cancellationToken);

            return MediatedResponse.Success();
        }
    }
}