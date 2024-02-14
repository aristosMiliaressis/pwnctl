using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using Microsoft.EntityFrameworkCore;
using MediatR;
using pwnctl.app;
using pwnctl.app.Tasks.Enums;
using pwnctl.domain.Entities;
using pwnctl.domain.ValueObjects;
using pwnctl.dto.Operations.Queries;
using pwnctl.app.Common.ValueObjects;
using pwnctl.dto.Operations.Models;

namespace pwnctl.api.Mediator.Handlers.Targets.Commands
{
    public sealed class OperationSummaryQueryHandler : IRequestHandler<OperationSummaryQuery, MediatedResponse<SummaryViewModel>>
    {
        public async Task<MediatedResponse<SummaryViewModel>> Handle(OperationSummaryQuery command, CancellationToken cancellationToken)
        {
            var viewModel = new SummaryViewModel();

            PwnctlDbContext context = new();
            var op = context.Operations
                            .Include(o => o.Scope)
                            .ThenInclude(o => o.Definitions)
                            .ThenInclude(o => o.Definition)
                            .FirstOrDefault(o => o.Name == ShortName.Create(command.Name));
            if (op == null)
                return MediatedResponse<SummaryViewModel>.Error("Operation {0} not found.", command.Name);

            viewModel.Name = op.Name.Value;
            viewModel.Type = op.Type;
            viewModel.State = op.State;
            viewModel.ScopeName = op.Scope.Name.Value;
            viewModel.CurrentPhase = op.CurrentPhase;
            viewModel.InitializedAt = op.InitiatedAt;
            viewModel.FinishedAt = op.FinishedAt;

            var scopeDefinitionIds = op.Scope.Definitions.Select(s => s.DefinitionId).ToList();

            viewModel.TagCount = await context.Tags.CountAsync();
            viewModel.InScopeRangesCount = await context.AssetRecords.Where(r => scopeDefinitionIds.Contains(r.ScopeId.Value) && r.NetworkRangeId != null).CountAsync();
            viewModel.InScopeHostCount = await context.AssetRecords.Where(r => scopeDefinitionIds.Contains(r.ScopeId.Value) && r.NetworkHostId != null).CountAsync();
            viewModel.InScopeDomainCount = await context.AssetRecords.Where(r => scopeDefinitionIds.Contains(r.ScopeId.Value) && r.DomainNameId != null).CountAsync();
            viewModel.InScopeRecordCount = await context.AssetRecords.Where(r => scopeDefinitionIds.Contains(r.ScopeId.Value) && r.DomainNameRecordId != null).CountAsync();
            viewModel.InScopeServiceCount = await context.AssetRecords.Where(r => scopeDefinitionIds.Contains(r.ScopeId.Value) && r.NetworkSocketId != null).CountAsync();
            viewModel.InScopeEndpointCount = await context.AssetRecords.Where(r => scopeDefinitionIds.Contains(r.ScopeId.Value) && r.HttpEndpointId != null).CountAsync();
            viewModel.InScopeParamCount = await context.AssetRecords.Where(r => scopeDefinitionIds.Contains(r.ScopeId.Value) && r.HttpParameterId != null).CountAsync();
            viewModel.InScopeVirtualHostCount = await context.AssetRecords.Where(r => scopeDefinitionIds.Contains(r.ScopeId.Value) && r.VirtualHostId != null).CountAsync();
            viewModel.InScopeEmailCount = await context.AssetRecords.Where(r => scopeDefinitionIds.Contains(r.ScopeId.Value) && r.EmailId != null).CountAsync();

            viewModel.QueuedTaskCount = await context.TaskRecords.Where(t => t.OperationId == op.Id && t.State == TaskState.QUEUED).CountAsync();
            viewModel.RunningTaskCount = await context.TaskRecords.Where(t => t.OperationId == op.Id && t.State == TaskState.RUNNING).CountAsync();
            viewModel.FinishedTaskCount = await context.TaskRecords.Where(t => t.OperationId == op.Id && t.State == TaskState.FINISHED).CountAsync();
            viewModel.FailedTaskCount = await context.TaskRecords.Where(t => t.OperationId == op.Id && t.State == TaskState.FAILED).CountAsync();
            viewModel.CanceledTaskCount = await context.TaskRecords.Where(t => t.OperationId == op.Id && t.State == TaskState.CANCELED).CountAsync();
            viewModel.TimedOutTaskCount = await context.TaskRecords.Where(t => t.OperationId == op.Id && t.State == TaskState.TIMED_OUT).CountAsync();
            viewModel.FirstTask = (await context.TaskRecords.Where(t => t.OperationId == op.Id).OrderBy(t => t.QueuedAt).FirstOrDefaultAsync())?.QueuedAt;
            viewModel.LastTask = (await context.TaskRecords.Where(t => t.OperationId == op.Id).OrderByDescending(t => t.QueuedAt).FirstOrDefaultAsync())?.QueuedAt;
            viewModel.LastFinishedTask = (await context.TaskRecords.Where(t => t.OperationId == op.Id).OrderByDescending(t => t.FinishedAt).FirstOrDefaultAsync())?.FinishedAt;
            viewModel.TaskDetails = new List<SummaryViewModel.TaskDefinitionDetails>();
            foreach (var def in context.TaskDefinitions.AsEnumerable().GroupBy(d => d.Name.Value).ToList())
            {
                var details = new SummaryViewModel.TaskDefinitionDetails
                {
                    Name = def.First().Name.Value,
                    ShortLived = def.First().ShortLived,
                    Count = await context.TaskRecords
                                        .Where(t => t.OperationId == op.Id && def.Select(d => d.Id).Contains(t.DefinitionId))
                                        .CountAsync(),
                    RunCount = context.TaskRecords
                                        .Where(t => t.OperationId == op.Id && def.Select(d => d.Id).Contains(t.DefinitionId))
                                        .Sum(e => e.RunCount),
                    Findings = await context.AssetRecords
                                        .Include(r => r.FoundByTask)
                                        .Where(r => scopeDefinitionIds.Contains(r.ScopeId.Value) 
                                                && def.Select(d => d.Id).Contains(r.FoundByTask.DefinitionId))
                                        .CountAsync()
                };

                if (details.Count == 0)
                    continue;

                var count = await context.TaskRecords
                                            .Where(e => e.OperationId == op.Id 
                                                    && e.State == TaskState.FINISHED 
                                                    && def.Select(d => d.Id).Contains(e.DefinitionId))
                                            .CountAsync();

                for (var i = 0; i <= count / PwnInfraContext.Config.Api.BatchSize; i++)
                {
                    details.Duration += TimeSpan.FromSeconds(context.TaskRecords
                            .Where(e => e.OperationId == op.Id 
                                    && e.State == TaskState.FINISHED 
                                    && def.Select(d => d.Id).Contains(e.DefinitionId))
                            .OrderBy(t => t.QueuedAt)
                            .Skip(i*PwnInfraContext.Config.Api.BatchSize)
                            .Take(PwnInfraContext.Config.Api.BatchSize)
                            .Select(e => e.FinishedAt - e.StartedAt).Sum(e => e.TotalSeconds));
                }

                viewModel.TaskDetails.Add(details);
            }

            return MediatedResponse<SummaryViewModel>.Success(viewModel);
        }
    }
}
