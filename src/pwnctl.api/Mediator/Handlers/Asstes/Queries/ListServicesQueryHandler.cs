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
    public sealed class ListServicesQueryHandler : IRequestHandler<ListServicesQuery, MediatedResponse<ServiceListViewModel>>
    {
        public async Task<MediatedResponse<ServiceListViewModel>> Handle(ListServicesQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var services = await repository.ListServicesAsync(query.Page);

            var viewModel = new ServiceListViewModel(services);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().NetworkSockets.Count() / PwnInfraContext.Config.Api.BatchSize;

            return MediatedResponse<ServiceListViewModel>.Success(viewModel);
        }
    }
}