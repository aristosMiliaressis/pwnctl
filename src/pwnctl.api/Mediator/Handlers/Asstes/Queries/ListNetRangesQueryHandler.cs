using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.app.Common;
using pwnctl.app;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.Models;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListNetRangesQueryHandler : IRequestHandler<ListNetRangesQuery, MediatedResponse<NetworkRangeListViewModel>>
    {
        public async Task<MediatedResponse<NetworkRangeListViewModel>> Handle(ListNetRangesQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var netRanges = await repository.ListNetworkRangesAsync(query.Page);

            var viewModel = new NetworkRangeListViewModel(netRanges);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().NetworkRanges.Count() / PwnInfraContext.Config.Api.BatchSize;

            return MediatedResponse<NetworkRangeListViewModel>.Success(viewModel);
        }
    }
}