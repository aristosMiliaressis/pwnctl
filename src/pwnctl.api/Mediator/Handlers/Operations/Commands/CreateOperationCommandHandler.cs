using Microsoft.EntityFrameworkCore;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.dto.Operations.Commands;

using MediatR;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.infra;
using pwnctl.infra.Scheduling;
using pwnctl.api.Mediator.Handlers.Scope.Commands;
using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Assets.DTO;
using pwnctl.app.Assets;
using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets.Aggregates;
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
        private readonly EventBridgeScheduler _scheduler = new();
        private readonly AssetRepository _assetRepository;
        private readonly TaskRepository _taskRepository;
        private readonly TaskQueueService _taskQueueService;

        public CreateOperationCommandHandler(AssetRepository assetRepo, TaskRepository taskRepo, TaskQueueService taskQueueSrv)
        {
            _assetRepository = assetRepo;
            _taskRepository = taskRepo;
            _taskQueueService = taskQueueSrv;
        }

        public async Task<MediatedResponse> Handle(CreateOperationCommand command, CancellationToken cancellationToken)
        {
            var existingOperation = await _context.Operations.FirstOrDefaultAsync(p => p.ShortName == ShortName.Create(command.ShortName));
            if (existingOperation != null)
                return MediatedResponse.Error("Operation {0} already exists.", command.ShortName);

            var scopeAggregate = _context.ScopeAggregates
                                        .Include(a => a.Definitions)
                                            .ThenInclude(d => d.Definition)
                                        .FirstOrDefault(a => a.ShortName == ShortName.Create(command.Scope.ShortName));
            if (scopeAggregate == null)
            {
                if (command.Scope.ScopeDefinitions == null || !command.Scope.ScopeDefinitions.Any())
                    return MediatedResponse.Error("Scope Aggregate {0} not found.", command.Scope.ShortName);

                scopeAggregate = await CreateScopeAggregateCommandHandler.CreateScopeAggregate(_context, command.Scope, cancellationToken);
            }

            var taskProfile = _context.TaskProfiles
                                        .Include(p => p.TaskDefinitions)
                                        .FirstOrDefault(p => p.ShortName == ShortName.Create(command.Policy.TaskProfile));
            if (taskProfile == null)
                return MediatedResponse.Error("Task Profile {0} not found.", command.Policy.TaskProfile);

            var policy = new Policy(taskProfile);
            policy.Whitelist = command.Policy.Whitelist;
            policy.Blacklist = command.Policy.Blacklist;
            policy.MaxAggressiveness = command.Policy.MaxAggressiveness;
            policy.OnlyPassive = command.Policy.OnlyPassive;

            var op = new Operation(command.ShortName, command.Type, policy, scopeAggregate);
            if (command.CronSchedule != null)
                op.Schedule = CronExpression.Create(command.CronSchedule);

            _context.Operations.Add(op);
            if (command.Type == OperationType.Crawl)
                op.State = OperationState.Ongoing;

            await _context.SaveChangesAsync();

            if (command.Type == OperationType.Crawl)
            {
                await StartCrawlOperation(op, command.Input);
            }
            else if (command.Type == OperationType.Scan || command.CronSchedule != null)
            {
                await _scheduler.ScheduleOperation(op);
            }

            return MediatedResponse.Success();
        }

        private async Task StartCrawlOperation(Operation op, IEnumerable<string> input)
        {
            var processor = AssetProcessorFactory.Create();

            foreach (var assetText in input.Where(a => !string.IsNullOrEmpty(a)))
            {
                AssetDTO dto = TagParser.Parse(assetText);

                Asset asset = AssetParser.Parse(dto.Asset);

                AssetRecord record = new(asset);


                var scope = op.Scope.Definitions.FirstOrDefault(scope => scope.Definition.Matches(record.Asset));
                if (scope != null)
                    record.SetScopeId(scope.Definition.Id);

                var outOfScopeTasks = _taskRepository.ListOutOfScope();
                var allowedTasks = op.Policy.GetAllowedTasks();
                allowedTasks.AddRange(outOfScopeTasks);

                foreach (var definition in allowedTasks.Where(def => (record.InScope || def.MatchOutOfScope) && def.Matches(record)))
                {
                    // only queue tasks once per definition/asset pair
                    var task = _taskRepository.Find(record, definition);
                    if (task != null)
                        continue;

                    task = new TaskRecord(op, definition, record);
                    record.Tasks.Add(task);
                }

                await _assetRepository.SaveAsync(record);

                foreach (var task in record.Tasks)
                {
                    task.Definition = allowedTasks.First(t => t.Id == task.DefinitionId);
                    await _taskQueueService.EnqueueAsync(new PendingTaskDTO(task));
                }
            }
        }
    }
}
