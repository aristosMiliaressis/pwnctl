using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.app.Common;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.Models;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListNetRangesQueryHandler : IRequestHandler<ListNetRangesQuery, MediatedResponse<NetRangeListViewModel>>
    {
        public async Task<MediatedResponse<NetRangeListViewModel>> Handle(ListNetRangesQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var netRanges = await repository.ListNetRangesAsync(query.Page);

            var viewModel = new NetRangeListViewModel(netRanges);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().NetworkRanges.Count() / Constants.BATCH_SIZE;

            return MediatedResponse<NetRangeListViewModel>.Success(viewModel);
        }
    }
}