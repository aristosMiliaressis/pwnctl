using Microsoft.EntityFrameworkCore;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.dto.Operations.Commands;

using MediatR;
using pwnctl.kernel;
using pwnctl.kernel.BaseClasses;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.infra;
using pwnctl.infra.Repositories;
using pwnctl.infra.Scheduling;
using pwnctl.api.Mediator.Handlers.Scope.Commands;
using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Assets.DTO;
using pwnctl.app.Assets;
using pwnctl.app.Tagging;
using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets.Entities;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Tasks.Interfaces;

namespace pwnctl.api.Mediator.Handlers.Operations.Commands
{
    public sealed class CreateOperationCommandHandler : IRequestHandler<CreateOperationCommand, MediatedResponse>
    {
        private readonly PwnctlDbContext _context = new();
        private readonly EventBridgeClient _client = new();
        private readonly AssetRepository _assetRepository;
        private readonly TaskRepository _taskRepository;
        private readonly TaskQueueService _taskQueueService;

        public CreateOperationCommandHandler(TaskQueueService taskQueueSrv)
        {
            _assetRepository = new AssetDbRepository(_context);
            _taskRepository = new TaskDbRepository(_context);
            _taskQueueService = taskQueueSrv;
        }

        public async Task<MediatedResponse> Handle(CreateOperationCommand command, CancellationToken cancellationToken)
        {
            var existingOperation = await _context.Operations.FirstOrDefaultAsync(p => p.Name == ShortName.Create(command.Name));
            if (existingOperation is not null)
                return MediatedResponse.Error("Operation {0} already exists.", command.Name);

            var scopeAggregate = _context.ScopeAggregates
                                        .Include(a => a.Definitions)
                                            .ThenInclude(d => d.Definition)
                                        .FirstOrDefault(a => a.Name == ShortName.Create(command.Scope.Name));
            if (scopeAggregate is null)
            {
                if (command.Scope.ScopeDefinitions is null || !command.Scope.ScopeDefinitions.Any())
                    return MediatedResponse.Error("Scope Aggregate {0} not found.", command.Scope.Name);

                scopeAggregate = await CreateScopeAggregateCommandHandler.CreateScopeAggregate(_context, command.Scope, cancellationToken);
            }

            List<TaskProfile> taskProfiles = new();
            foreach (var profileName in command.Policy.TaskProfiles)
            {
                var profile = _context.TaskProfiles
                                        .Include(p => p.TaskDefinitions)
                                        .FirstOrDefault(p => p.Name == ShortName.Create(profileName));
                if (profile is null)
                    return MediatedResponse.Error("Task Profile {0} not found.", profileName);
                
                taskProfiles.Add(profile);
            }

            var policy = new Policy(taskProfiles);
            policy.Blacklist = command.Policy.Blacklist;

            var op = new Operation(command.Name, command.Type, policy, scopeAggregate);
            if (command.CronSchedule is not null)
                op.Schedule = CronExpression.Create(command.CronSchedule);

            _context.Operations.Add(op);
            await _context.SaveChangesAsync();

            if (command.Type == OperationType.Crawl)
            {
                await StartCrawlOperation(op, command.Input);
            }
            else if (command.Type == OperationType.Scan || command.CronSchedule is not null)
            {
                await _client.Schedule(op);
            }

            return MediatedResponse.Success();
        }

        private async Task StartCrawlOperation(Operation op, IEnumerable<string> input)
        {
            op.Initialize();
            await _context.SaveChangesAsync();

            foreach (var assetText in input.Where(a => !string.IsNullOrEmpty(a)))
            {
                Result<Asset, string> result = AssetParser.Parse(assetText);
                if (result.Failed)
                    continue;

                AssetRecord record = new(result.Value);

                var scope = op.Scope.Definitions.FirstOrDefault(scope => scope.Definition.Matches(record.Asset));
                if (scope is not null)
                    record.SetScopeId(scope.Definition.Id);

                var allowedTasks = op.Policy.GetAllowedTasks();

                foreach (var definition in allowedTasks.Where(def => (record.InScope || def.MatchOutOfScope) && def.Matches(record)))
                {
                    // only queue tasks once per definition/asset pair
                    var task = _taskRepository.Find(record, definition);
                    if (task is not null)
                        continue;

                    task = new TaskRecord(op, definition, record);
                    record.Tasks.Add(task);
                }

                await _assetRepository.SaveAsync(record);

                record.Tasks.ForEach(task => task.Definition = allowedTasks.First(t => t.Id == task.DefinitionId));

                var newTasks = record.Tasks.Where(t => t.Definition.Profile.Phase <= op.CurrentPhase);

                await _taskQueueService.EnqueueBatchAsync(newTasks.Where(t => t.Definition.ShortLived).Select(t => new ShortLivedTaskDTO(t)));

                await _taskQueueService.EnqueueBatchAsync(newTasks.Where(t => !t.Definition.ShortLived).Select(t => new LongLivedTaskDTO(t)));
            }

            await _client.Subscribe(op);
        }
    }
}
