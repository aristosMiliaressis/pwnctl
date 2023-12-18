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
    public sealed class ListNetworkHostsQueryHandler : IRequestHandler<ListNetworkHostsQuery, MediatedResponse<NetworkHostListViewModel>>
    {
        public async Task<MediatedResponse<NetworkHostListViewModel>> Handle(ListNetworkHostsQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var hosts = await repository.ListNetworkHostsAsync(query.Page);

            var viewModel = new NetworkHostListViewModel(hosts);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().NetworkHosts.Count() / PwnInfraContext.Config.Api.BatchSize;

            return MediatedResponse<NetworkHostListViewModel>.Success(viewModel);
        }
    }
}