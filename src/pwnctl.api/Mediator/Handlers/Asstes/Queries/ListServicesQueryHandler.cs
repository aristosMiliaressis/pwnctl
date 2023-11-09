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
    public sealed class ListServicesQueryHandler : IRequestHandler<ListServicesQuery, MediatedResponse<NetworkSocketListViewModel>>
    {
        public async Task<MediatedResponse<NetworkSocketListViewModel>> Handle(ListServicesQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var services = await repository.ListNetworkSocketsAsync(query.Page);

            var viewModel = new NetworkSocketListViewModel(services);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().NetworkSockets.Count() / PwnInfraContext.Config.Api.BatchSize;

            return MediatedResponse<NetworkSocketListViewModel>.Success(viewModel);
        }
    }
}