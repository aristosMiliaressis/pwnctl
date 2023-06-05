using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.Models;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListEndpointsQueryHandler : IRequestHandler<ListEndpointsQuery, MediatedResponse<EndpointListViewModel>>
    {
        public async Task<MediatedResponse<EndpointListViewModel>> Handle(ListEndpointsQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();
            
            var endpoints = await repository.ListEndpointsAsync(query.Page);

            var viewModel = new EndpointListViewModel(endpoints);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().HttpEndpoints.Count() / 4096;

            return MediatedResponse<EndpointListViewModel>.Success(viewModel);
        }
    }
}