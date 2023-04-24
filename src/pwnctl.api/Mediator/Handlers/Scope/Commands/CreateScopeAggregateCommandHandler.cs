using pwnctl.app.Scope.Entities;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.dto.Scope.Commands;
using pwnctl.app.Common.ValueObjects;
using pwnctl.dto.Scope.Models;

namespace pwnctl.api.Mediator.Handlers.Scope.Commands
{
    public sealed class CreateScopeAggregateCommandHandler : IRequestHandler<CreateScopeAggregateCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse> Handle(CreateScopeAggregateCommand command, CancellationToken cancellationToken)
        {
            var scopeAggregate = _context.ScopeAggregates.FirstOrDefault(a => a.ShortName == ShortName.Create(command.ShortName));
            if (scopeAggregate != null)
                return MediatedResponse.Error("Scope Aggregate {0} already exists.", command.ShortName);

            await CreateScopeAggregate(_context, command, cancellationToken);

            return MediatedResponse.Success();
        }

        public static async Task<ScopeAggregate> CreateScopeAggregate(PwnctlDbContext context, ScopeRequestModel command, CancellationToken cancellationToken = default)
        {
            var scopeAggregate = new ScopeAggregate(command.ShortName, command.Description);

            foreach (var def in command.ScopeDefinitions)
            {
                var scopeDef = context.ScopeDefinitions.FirstOrDefault(d => d.Pattern == def.Pattern && d.Type == def.Type);
                if (scopeDef == null)
                {
                    scopeDef = new ScopeDefinition(def.Type, def.Pattern);

                    context.ScopeDefinitions.Add(scopeDef);
                }

                scopeAggregate.Definitions.Add(new ScopeDefinitionAggregate(scopeAggregate, scopeDef));
            }

            context.Add(scopeAggregate);
            await context.SaveChangesAsync(cancellationToken);

            return scopeAggregate;
        }
    }
}