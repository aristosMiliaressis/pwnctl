using pwnctl.dto.Tasks.Queries;
using pwnctl.dto.Tasks.Models;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using Microsoft.EntityFrameworkCore;
using pwnctl.infra.Repositories;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListTaskEntriesQueryHandler : IRequestHandler<ListTaskEntriesQuery, MediatedResponse<TaskEntryListViewModel>>
    {
        private readonly TaskDbRepository _repo = new();

        public async Task<MediatedResponse<TaskEntryListViewModel>> Handle(ListTaskEntriesQuery query, CancellationToken cancellationToken)
        {
            var tasks = await _repo.ListEntriesAsync(query.Page);

            var viewModel = new TaskEntryListViewModel(tasks);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().TaskEntries.Count() / 4096;

            return MediatedResponse<TaskEntryListViewModel>.Success(viewModel);
        }
    }
}