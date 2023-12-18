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
    public sealed class ListHttpEndpointsQueryHandler : IRequestHandler<ListHttpEndpointsQuery, MediatedResponse<HttpEndpointListViewModel>>
    {
        public async Task<MediatedResponse<HttpEndpointListViewModel>> Handle(ListHttpEndpointsQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();
            
            var endpoints = await repository.ListHttpEndpointsAsync(query.Page);

            var viewModel = new HttpEndpointListViewModel(endpoints);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().HttpEndpoints.Count() / PwnInfraContext.Config.Api.BatchSize;

            return MediatedResponse<HttpEndpointListViewModel>.Success(viewModel);
        }
    }
}