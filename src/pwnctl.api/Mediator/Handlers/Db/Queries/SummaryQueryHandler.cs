using pwnctl.dto.Mediator;
using pwnctl.dto.Db.Queries;
using pwnctl.dto.Db.Models;
using pwnctl.infra.Persistence;

using Microsoft.EntityFrameworkCore;
using MediatR;
using pwnctl.app.Tasks.Enums;
using pwnctl.domain.Entities;

namespace pwnctl.api.Mediator.Handlers.Targets.Commands
{
    public sealed class SummaryQueryHandler : IRequestHandler<SummaryQuery, MediatedResponse<SummaryViewModel>>
    {
        public async Task<MediatedResponse<SummaryViewModel>> Handle(SummaryQuery command, CancellationToken cancellationToken)
        {
            var viewModel = new SummaryViewModel();

            PwnctlDbContext context = new();

            viewModel.NetworkRangeCount = await context.NetworkRanges.CountAsync();
            viewModel.HostCount = await context.Hosts.CountAsync();
            viewModel.DomainCount = await context.Domains.CountAsync();
            viewModel.RecordCount = await context.DNSRecords.CountAsync();
            viewModel.SocketCount = await context.Sockets.CountAsync();
            viewModel.HttpEndpointCount = await context.HttpEndpoints.CountAsync();
            viewModel.HttpParamCount = await context.HttpParameters.CountAsync();
            viewModel.EmailCount = await context.Emails.CountAsync();
            viewModel.TagCount = await context.Tags.CountAsync();
            viewModel.InScopeRangesCount = await context.AssetRecords.Where(r => r.SubjectClass.Value == nameof(NetworkRange) && r.InScope).CountAsync();
            viewModel.InScopeHostCount = await context.AssetRecords.Where(r => r.SubjectClass.Value == nameof(NetworkHost) && r.InScope).CountAsync();
            viewModel.InScopeDomainCount = await context.AssetRecords.Where(r => r.SubjectClass.Value == nameof(DomainName) && r.InScope).CountAsync();
            viewModel.InScopeRecordCount = await context.AssetRecords.Where(r => r.SubjectClass.Value == nameof(DomainNameRecord) && r.InScope).CountAsync();
            viewModel.InScopeServiceCount = await context.AssetRecords.Where(r => r.SubjectClass.Value == nameof(NetworkSocket) && r.InScope).CountAsync();
            viewModel.InScopeEndpointCount = await context.AssetRecords.Where(r => r.SubjectClass.Value == nameof(domain.Entities.HttpEndpoint) && r.InScope).CountAsync();
            viewModel.InScopeParamCount = await context.AssetRecords.Where(r => r.SubjectClass.Value == nameof(HttpParameter) && r.InScope).CountAsync();
            viewModel.InScopeEmailCount = await context.AssetRecords.Where(r => r.SubjectClass.Value == nameof(Email) && r.InScope).CountAsync();

            viewModel.PendingTaskCount = await context.TaskEntries.Where(t => t.State == TaskState.PENDING).CountAsync();
            viewModel.QueuedTaskCount = await context.TaskEntries.Where(t => t.State == TaskState.QUEUED).CountAsync();
            viewModel.RunningTaskCount = await context.TaskEntries.Where(t => t.State == TaskState.RUNNING).CountAsync();
            viewModel.FinishedTaskCount = await context.TaskEntries.Where(t => t.State == TaskState.FINISHED).CountAsync();
            viewModel.FirstTask = (await context.TaskEntries.Where(t => t.State != TaskState.PENDING).OrderBy(t => t.QueuedAt).FirstOrDefaultAsync())?.QueuedAt;
            viewModel.LastTask = (await context.TaskEntries.OrderByDescending(t => t.QueuedAt).FirstOrDefaultAsync())?.QueuedAt;
            viewModel.LastFinishedTask = (await context.TaskEntries.OrderByDescending(t => t.FinishedAt).FirstOrDefaultAsync())?.FinishedAt;
            viewModel.TaskDetails = new List<SummaryViewModel.TaskDefinitionDetails>();
            foreach (var def in context.TaskDefinitions.ToList())
            {
                var entries = context.TaskEntries.Where(e => e.DefinitionId == def.Id).ToList();
                viewModel.TaskDetails.Add(new SummaryViewModel.TaskDefinitionDetails
                {
                    ShortName = def.ShortName.Value,
                    Count = entries.Count,
                    Duration = TimeSpan.FromSeconds(entries.Where(e => e.State == TaskState.FINISHED).Select(e => e.FinishedAt - e.StartedAt).Sum(e => e.TotalSeconds)),
                    Findings = context.AssetRecords.Include(r => r.FoundByTask).Where(r => r.FoundByTask.DefinitionId == def.Id).Count()
                });
            }

            return MediatedResponse<SummaryViewModel>.Success(viewModel);
        }
    }
}