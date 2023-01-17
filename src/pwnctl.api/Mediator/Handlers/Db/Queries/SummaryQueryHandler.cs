using pwnctl.dto.Mediator;
using pwnctl.dto.Db.Queries;
using pwnctl.dto.Db.ViewModels;
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

            viewModel.NetRangeCount = await context.NetRanges.CountAsync();
            viewModel.HostCount = await context.Hosts.CountAsync();
            viewModel.DomainCount = await context.Domains.CountAsync();
            viewModel.RecordCount = await context.DNSRecords.CountAsync();
            viewModel.ServiceCount = await context.Services.CountAsync();
            viewModel.EndpointCount = await context.Endpoints.CountAsync();
            viewModel.ParamCount = await context.Parameters.CountAsync();
            viewModel.EmailCount = await context.Emails.CountAsync();
            viewModel.TagCount = await context.Tags.CountAsync();
            viewModel.InScopeRangesCount = await context.AssetRecords.Where(r => r.SubjectClass.Class == nameof(NetRange) && r.InScope).CountAsync();
            viewModel.InsCopeHostCount = await context.Hosts.CountAsync();
            viewModel.InScopeDomainCount = await context.AssetRecords.Where(r => r.SubjectClass.Class == nameof(Domain) && r.InScope).CountAsync();
            viewModel.InScopeRecordCount = await context.AssetRecords.Where(r => r.SubjectClass.Class == nameof(DNSRecord) && r.InScope).CountAsync();
            viewModel.InScopeServiceCount = await context.AssetRecords.Where(r => r.SubjectClass.Class == nameof(Service) && r.InScope).CountAsync();
            viewModel.InScopeEndpointCount = await context.AssetRecords.Where(r => r.SubjectClass.Class == nameof(domain.Entities.Endpoint) && r.InScope).CountAsync();
            viewModel.InScopeParamCount = await context.AssetRecords.Where(r => r.SubjectClass.Class == nameof(Parameter) && r.InScope).CountAsync();
            viewModel.InScopeEmailCount = await context.AssetRecords.Where(r => r.SubjectClass.Class == nameof(Email) && r.InScope).CountAsync();

            viewModel.PendingTaskCount = await context.TaskEntries.Where(t => t.State == TaskState.PENDING).CountAsync();
            viewModel.QueuedTaskCount = await context.TaskEntries.Where(t => t.State == TaskState.QUEUED).CountAsync();
            viewModel.RunningTaskCount = await context.TaskEntries.Where(t => t.State == TaskState.RUNNING).CountAsync();
            viewModel.FinishedTaskCount = await context.TaskEntries.Where(t => t.State == TaskState.FINISHED).CountAsync();
            viewModel.FirstTask = (await context.TaskEntries.Where(t => t.State != TaskState.PENDING).OrderBy(t => t.QueuedAt).FirstOrDefaultAsync())?.QueuedAt;
            viewModel.LastTask = (await context.TaskEntries.Where(t => t.State != TaskState.PENDING).OrderByDescending(t => t.QueuedAt).FirstOrDefaultAsync())?.QueuedAt;
            viewModel.TaskDetails = new List<SummaryViewModel.TaskDefinitionDetails>();
            foreach (var def in context.TaskDefinitions.ToList())
            {
                var entries = context.TaskEntries.Where(e => e.State == TaskState.FINISHED && e.DefinitionId == def.Id).ToList();
                viewModel.TaskDetails.Add(new SummaryViewModel.TaskDefinitionDetails
                {
                    ShortName = def.ShortName,
                    Count = entries.Count,
                    Duration = TimeSpan.FromSeconds(entries.Select(e => e.FinishedAt - e.StartedAt).Sum(e => e.TotalSeconds))
                });
            }

            return MediatedResponse<SummaryViewModel>.Success(viewModel);
        }
    }
}