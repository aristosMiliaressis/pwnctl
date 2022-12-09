using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.ViewModels;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListEndpointsQueryHandler : IRequestHandler<ListEndpointsQuery, MediatedResponse<EndpointListViewModel>>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse<EndpointListViewModel>> Handle(ListEndpointsQuery command, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var endpoints = await repository.ListEndpointsAsync();

            return MediatedResponse<EndpointListViewModel>.Success(new EndpointListViewModel(endpoints));
        }
    }
}