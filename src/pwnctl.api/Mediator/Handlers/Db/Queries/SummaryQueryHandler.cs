using pwnctl.dto.Mediator;
using pwnctl.dto.Db.Queries;
using pwnctl.dto.Db.ViewModels;
using pwnctl.infra.Persistence;

using Microsoft.EntityFrameworkCore;
using MediatR;
using pwnctl.app.Tasks.Enums;

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
            viewModel.InScopeRangesCount = await context.NetRanges.Where(a => a.InScope).CountAsync();
            viewModel.InsCopeHostCount = await context.Hosts.CountAsync();
            viewModel.InScopeDomainCount = await context.Domains.Where(a => a.InScope).CountAsync();
            viewModel.InScopeRecordCount = await context.DNSRecords.Where(a => a.InScope).CountAsync();
            viewModel.InScopeServiceCount = await context.Services.Where(a => a.InScope).CountAsync();
            viewModel.InScopeEndpointCount = await context.Endpoints.Where(a => a.InScope).CountAsync();
            viewModel.InScopeParamCount = await context.Parameters.Where(a => a.InScope).CountAsync();
            viewModel.InScopeEmailCount = await context.Emails.Where(a => a.InScope).CountAsync();
            viewModel.FirstTask = (await context.TaskRecords.Where(t => t.State != TaskState.PENDING).OrderBy(t => t.QueuedAt).FirstOrDefaultAsync())?.QueuedAt;
            viewModel.LastTask = (await context.TaskRecords.Where(t => t.State != TaskState.PENDING).OrderByDescending(t => t.QueuedAt).FirstOrDefaultAsync())?.QueuedAt;

            return MediatedResponse<SummaryViewModel>.Success(viewModel);
        }
    }
}