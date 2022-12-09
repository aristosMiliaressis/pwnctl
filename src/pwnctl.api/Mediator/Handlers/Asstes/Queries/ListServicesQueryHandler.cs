using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.ViewModels;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListServicesQueryHandler : IRequestHandler<ListServicesQuery, MediatedResponse<ServiceListViewModel>>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse<ServiceListViewModel>> Handle(ListServicesQuery command, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var services = await repository.ListServicesAsync();

            return MediatedResponse<ServiceListViewModel>.Success(new ServiceListViewModel(services));
        }
    }
}