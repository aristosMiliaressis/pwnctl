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
            var op = context.Operations.FirstOrDefault(o => o.Name == ShortName.Create(command.Name));
            if (op == null)
                return MediatedResponse<SummaryViewModel>.Error("Operation {0} not found.", command.Name);

            viewModel.NetworkRangeCount = await context.NetworkRanges.CountAsync();
            viewModel.HostCount = await context.NetworkHosts.CountAsync();
            viewModel.DomainCount = await context.DomainNames.CountAsync();
            viewModel.RecordCount = await context.DomainNameRecords.CountAsync();
            viewModel.SocketCount = await context.NetworkSockets.CountAsync();
            viewModel.HttpEndpointCount = await context.HttpEndpoints.CountAsync();
            viewModel.HttpParamCount = await context.HttpParameters.CountAsync();
            viewModel.EmailCount = await context.Emails.CountAsync();
            viewModel.TagCount = await context.Tags.CountAsync();
            viewModel.InScopeRangesCount = await context.AssetRecords.Where(r => r.Subject == AssetClass.Create(nameof(NetworkRange)) && r.InScope).CountAsync();
            viewModel.InScopeHostCount = await context.AssetRecords.Where(r => r.Subject == AssetClass.Create(nameof(NetworkHost)) && r.InScope).CountAsync();
            viewModel.InScopeDomainCount = await context.AssetRecords.Where(r => r.Subject == AssetClass.Create(nameof(DomainName)) && r.InScope).CountAsync();
            viewModel.InScopeRecordCount = await context.AssetRecords.Where(r => r.Subject == AssetClass.Create(nameof(DomainNameRecord)) && r.InScope).CountAsync();
            viewModel.InScopeServiceCount = await context.AssetRecords.Where(r => r.Subject == AssetClass.Create(nameof(NetworkSocket)) && r.InScope).CountAsync();
            viewModel.InScopeEndpointCount = await context.AssetRecords.Where(r => r.Subject == AssetClass.Create(nameof(domain.Entities.HttpEndpoint)) && r.InScope).CountAsync();
            viewModel.InScopeParamCount = await context.AssetRecords.Where(r => r.Subject == AssetClass.Create(nameof(HttpParameter)) && r.InScope).CountAsync();
            viewModel.InScopeEmailCount = await context.AssetRecords.Where(r => r.Subject == AssetClass.Create(nameof(Email)) && r.InScope).CountAsync();

            viewModel.QueuedTaskCount = await context.TaskRecords.Where(t => t.State == TaskState.QUEUED).CountAsync();
            viewModel.RunningTaskCount = await context.TaskRecords.Where(t => t.State == TaskState.RUNNING).CountAsync();
            viewModel.FinishedTaskCount = await context.TaskRecords.Where(t => t.State == TaskState.FINISHED).CountAsync();
            viewModel.FailedTaskCount = await context.TaskRecords.Where(t => t.State == TaskState.FAILED).CountAsync();
            viewModel.CanceledTaskCount = await context.TaskRecords.Where(t => t.State == TaskState.CANCELED).CountAsync();
            viewModel.TimedOutTaskCount = await context.TaskRecords.Where(t => t.State == TaskState.TIMED_OUT).CountAsync();
            viewModel.FirstTask = (await context.TaskRecords.OrderBy(t => t.QueuedAt).FirstOrDefaultAsync())?.QueuedAt;
            viewModel.LastTask = (await context.TaskRecords.OrderByDescending(t => t.QueuedAt).FirstOrDefaultAsync())?.QueuedAt;
            viewModel.LastFinishedTask = (await context.TaskRecords.OrderByDescending(t => t.FinishedAt).FirstOrDefaultAsync())?.FinishedAt;
            viewModel.TaskDetails = new List<SummaryViewModel.TaskDefinitionDetails>();
            foreach (var def in context.TaskDefinitions.AsEnumerable().GroupBy(d => d.Name.Value).ToList())
            {
                var details = new SummaryViewModel.TaskDefinitionDetails
                {
                    Name = def.First().Name.Value,
                    Count = await context.TaskRecords.Where(e => def.Select(d => d.Id).Contains(e.DefinitionId)).CountAsync(),
                    RunCount = context.TaskRecords.Where(e => def.Select(d => d.Id).Contains(e.DefinitionId)).Sum(e => e.RunCount),
                    Findings = context.AssetRecords.Include(r => r.FoundByTask).Where(r => def.Select(d => d.Id).Contains(r.FoundByTask.DefinitionId)).Count()
                };

                if (details.Count == 0)
                    continue;

                var count = await  context.TaskRecords.Where(e => e.State == TaskState.FINISHED && def.Select(d => d.Id).Contains(e.DefinitionId)).CountAsync();
                for (var i = 0; i <= count / PwnInfraContext.Config.Api.BatchSize; i++)
                {
                    details.Duration += TimeSpan.FromSeconds(context.TaskRecords
                            .Where(e => e.State == TaskState.FINISHED && def.Select(d => d.Id).Contains(e.DefinitionId))
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
