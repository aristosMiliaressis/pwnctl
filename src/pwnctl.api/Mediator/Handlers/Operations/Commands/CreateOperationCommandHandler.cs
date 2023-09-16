using Microsoft.EntityFrameworkCore;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.dto.Operations.Commands;

using MediatR;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.infra;
using pwnctl.infra.Repositories;
using pwnctl.infra.Scheduling;
using pwnctl.api.Mediator.Handlers.Scope.Commands;
using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Assets.DTO;
using pwnctl.app.Assets;
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
        private readonly EventBridgeScheduler _scheduler = new();
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
            var existingOperation = await _context.Operations.FirstOrDefaultAsync(p => p.ShortName == ShortName.Create(command.ShortName));
            if (existingOperation is not null)
                return MediatedResponse.Error("Operation {0} already exists.", command.ShortName);

            var scopeAggregate = _context.ScopeAggregates
                                        .Include(a => a.Definitions)
                                            .ThenInclude(d => d.Definition)
                                        .FirstOrDefault(a => a.ShortName == ShortName.Create(command.Scope.ShortName));
            if (scopeAggregate is null)
            {
                if (command.Scope.ScopeDefinitions is null || !command.Scope.ScopeDefinitions.Any())
                    return MediatedResponse.Error("Scope Aggregate {0} not found.", command.Scope.ShortName);

                scopeAggregate = await CreateScopeAggregateCommandHandler.CreateScopeAggregate(_context, command.Scope, cancellationToken);
            }

            List<TaskProfile> taskProfiles = new();
            foreach (var profileName in command.Policy.TaskProfiles)
            {
                var profile = _context.TaskProfiles
                                        .Include(p => p.TaskDefinitions)
                                        .FirstOrDefault(p => p.ShortName == ShortName.Create(profileName));
                if (profile is null)
                    return MediatedResponse.Error("Task Profile {0} not found.", profileName);
                
                taskProfiles.Add(profile);
            }

            var policy = new Policy(taskProfiles);
            policy.Blacklist = command.Policy.Blacklist;

            var op = new Operation(command.ShortName, command.Type, policy, scopeAggregate);
            if (command.CronSchedule is not null)
                op.Schedule = CronExpression.Create(command.CronSchedule);

            _context.Operations.Add(op);
            if (command.Type == OperationType.Crawl)
                op.State = OperationState.Ongoing;

            await _context.SaveChangesAsync();

            if (command.Type == OperationType.Crawl)
            {
                await StartCrawlOperation(op, command.Input);
            }
            else if (command.Type == OperationType.Scan || command.CronSchedule is not null)
            {
                await _scheduler.ScheduleOperation(op);
            }

            return MediatedResponse.Success();
        }

        private async Task StartCrawlOperation(Operation op, IEnumerable<string> input)
        {
            foreach (var assetText in input.Where(a => !string.IsNullOrEmpty(a)))
            {
                AssetDTO dto = TagParser.Parse(assetText);

                Asset asset = AssetParser.Parse(dto.Asset);

                AssetRecord record = new(asset);

                var scope = op.Scope.Definitions.FirstOrDefault(scope => scope.Definition.Matches(record.Asset));
                if (scope is not null)
                    record.SetScopeId(scope.Definition.Id);

                var outOfScopeTasks = _taskRepository.ListOutOfScope();
                var allowedTasks = op.Policy.GetAllowedTasks();
                allowedTasks.AddRange(outOfScopeTasks);

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

                foreach (var task in record.Tasks)
                {
                    task.Definition = allowedTasks.First(t => t.Id == task.DefinitionId);
                    await _taskQueueService.EnqueueAsync(new PendingTaskDTO(task));
                }
            }
        }
    }
}
