using Microsoft.EntityFrameworkCore;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.dto.Operations.Commands;

using MediatR;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.infra.Repositories;
using pwnctl.infra;
using pwnctl.api.Mediator.Handlers.Scope.Commands;
using pwnctl.app;

namespace pwnctl.api.Mediator.Handlers.Operations.Commands
{
    public sealed class CreateOperationCommandHandler : IRequestHandler<CreateOperationCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse> Handle(CreateOperationCommand command, CancellationToken cancellationToken)
        {
            var existingProgram = await _context.Operations.FirstOrDefaultAsync(p => p.ShortName.Value == command.ShortName);
            if (existingProgram != null)
                return MediatedResponse.Error("Target {0} already exists.", command.ShortName);

            var scopeAggregate = _context.ScopeAggregates.FirstOrDefault(a => a.ShortName.Value == command.Scope.ShortName);
            if (scopeAggregate == null)
            {
                if (command.Scope.ScopeDefinitions == null || !command.Scope.ScopeDefinitions.Any())
                    return MediatedResponse.Error("Scope Aggregate {0} not found.", command.Scope.ShortName);

                scopeAggregate = await CreateScopeAggregateCommandHandler.CreateScopeAggregate(_context, command.Scope, cancellationToken);
            }

            var taskProfile = _context.TaskProfiles
                                        .Include(p => p.TaskDefinitions)
                                        .FirstOrDefault(p => p.ShortName.Value == command.Policy.TaskProfile);
            if (taskProfile == null)
                return MediatedResponse.Error("Task Profile {0} not found.", command.Policy.TaskProfile);

            var policy = new Policy(taskProfile);
            policy.Whitelist = command.Policy.Whitelist;
            policy.Blacklist = command.Policy.Blacklist;
            policy.MaxAggressiveness = command.Policy.MaxAggressiveness;
            policy.OnlyPassive = command.Policy.OnlyPassive;

            var op = new Operation(command.ShortName, command.Type, policy, scopeAggregate);

            _context.Operations.Add(op);
            await _context.SaveChangesAsync();

            if (command.Type == OperationType.Crawl)
            {
                await StartCrawlOperation(op, command.Input);
            }
            else if (command.Type == OperationType.Scan)
            {
                StartScanOperation(op);
            }

            return MediatedResponse.Success();
        }

        private async Task StartCrawlOperation(Operation op, IEnumerable<string> input)
        {
            var processor = AssetProcessorFactory.Create();

            foreach (var asset in input.Where(a => !string.IsNullOrEmpty(a)))
            {
                await processor.TryProcessAsync(asset, op);
            }
        }

        private void StartScanOperation(Operation op)
        {
            throw new NotImplementedException();
        }
    }
}