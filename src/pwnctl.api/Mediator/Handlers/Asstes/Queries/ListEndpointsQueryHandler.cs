using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.app.Common;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.Models;
using pwnctl.app;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListEndpointsQueryHandler : IRequestHandler<ListEndpointsQuery, MediatedResponse<EndpointListViewModel>>
    {
        public async Task<MediatedResponse<EndpointListViewModel>> Handle(ListEndpointsQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();
            
            PwnInfraContext.Logger.Warning("HERE");
            var endpoints = await repository.ListEndpointsAsync(query.Page);
            PwnInfraContext.Logger.Warning("HERE2");

            var viewModel = new EndpointListViewModel(endpoints);
            PwnInfraContext.Logger.Warning("HERE3");

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().HttpEndpoints.Count() / Constants.BATCH_SIZE;
            PwnInfraContext.Logger.Warning("HERE4");

            return MediatedResponse<EndpointListViewModel>.Success(viewModel);
        }
    }
}