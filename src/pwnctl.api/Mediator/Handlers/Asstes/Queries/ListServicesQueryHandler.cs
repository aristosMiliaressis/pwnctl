using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

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
            viewModel.TotalPages = new PwnctlDbContext().NetworkSockets.Count() / 4096;

            return MediatedResponse<ServiceListViewModel>.Success(viewModel);
        }
    }
}