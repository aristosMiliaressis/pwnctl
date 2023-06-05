using pwnctl.dto.Mediator;
using pwnctl.dto.Db.Queries;
using pwnctl.dto.Db.Models;
using pwnctl.infra.Persistence;

using Microsoft.EntityFrameworkCore;
using MediatR;
using pwnctl.app.Tasks.Enums;
using pwnctl.domain.Entities;
using pwnctl.domain.ValueObjects;

namespace pwnctl.api.Mediator.Handlers.Targets.Commands
{
    public sealed class SummaryQueryHandler : IRequestHandler<SummaryQuery, MediatedResponse<SummaryViewModel>>
    {
        public async Task<MediatedResponse<SummaryViewModel>> Handle(SummaryQuery command, CancellationToken cancellationToken)
        {
            var viewModel = new SummaryViewModel();

            PwnctlDbContext context = new();

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

            viewModel.QueuedTaskCount = await context.TaskEntries.Where(t => t.State == TaskState.QUEUED).CountAsync();
            viewModel.RunningTaskCount = await context.TaskEntries.Where(t => t.State == TaskState.RUNNING).CountAsync();
            viewModel.FinishedTaskCount = await context.TaskEntries.Where(t => t.State == TaskState.FINISHED).CountAsync();
            viewModel.FirstTask = (await context.TaskEntries.OrderBy(t => t.QueuedAt).FirstOrDefaultAsync())?.QueuedAt;
            viewModel.LastTask = (await context.TaskEntries.OrderByDescending(t => t.QueuedAt).FirstOrDefaultAsync())?.QueuedAt;
            viewModel.LastFinishedTask = (await context.TaskEntries.OrderByDescending(t => t.FinishedAt).FirstOrDefaultAsync())?.FinishedAt;
            viewModel.TaskDetails = new List<SummaryViewModel.TaskDefinitionDetails>();
            foreach (var def in context.TaskDefinitions.AsEnumerable().GroupBy(d => d.Name.Value).ToList())
            {
                var details = new SummaryViewModel.TaskDefinitionDetails
                {
                    ShortName = def.First().Name.Value,
                    Count = await context.TaskEntries.Where(e => def.Select(d => d.Id).Contains(e.DefinitionId)).CountAsync(),
                    Findings = context.AssetRecords.Include(r => r.FoundByTask).Where(r => def.Select(d => d.Id).Contains(r.FoundByTask.DefinitionId)).Count()
                };

                var count = await  context.TaskEntries.Where(e => e.State == TaskState.FINISHED && def.Select(d => d.Id).Contains(e.DefinitionId)).CountAsync();
                for (var i = 0; i <= count / 1024; i++)
                {
                    details.Duration += TimeSpan.FromSeconds(context.TaskEntries
                            .Where(e => e.State == TaskState.FINISHED && def.Select(d => d.Id).Contains(e.DefinitionId))
                            .OrderBy(t => t.QueuedAt)
                            .Skip(i*1024)
                            .Take(1024)
                            .Select(e => e.FinishedAt - e.StartedAt).Sum(e => e.TotalSeconds));
                }

                viewModel.TaskDetails.Add(details);
            }

            return MediatedResponse<SummaryViewModel>.Success(viewModel);
        }
    }
}
