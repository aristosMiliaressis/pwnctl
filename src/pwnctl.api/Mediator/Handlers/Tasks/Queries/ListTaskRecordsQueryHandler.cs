using pwnctl.dto.Tasks.Queries;
using pwnctl.dto.Tasks.Models;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.app.Common;
using pwnctl.app;

using MediatR;
using Microsoft.EntityFrameworkCore;
using pwnctl.infra.Repositories;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListTaskRecordsQueryHandler : IRequestHandler<ListTaskRecordsQuery, MediatedResponse<TaskRecordListViewModel>>
    {
        private readonly TaskDbRepository _repo = new();

        public async Task<MediatedResponse<TaskRecordListViewModel>> Handle(ListTaskRecordsQuery query, CancellationToken cancellationToken)
        {
            var tasks = await _repo.ListAsync(query.Page);

            var viewModel = new TaskRecordListViewModel(tasks);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().TaskRecords.Count() / PwnInfraContext.Config.Api.BatchSize;

            return MediatedResponse<TaskRecordListViewModel>.Success(viewModel);
        }
    }
}