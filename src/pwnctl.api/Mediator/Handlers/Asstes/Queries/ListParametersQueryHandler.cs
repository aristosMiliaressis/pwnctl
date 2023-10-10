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
    public sealed class ListParametersQueryHandler : IRequestHandler<ListParametersQuery, MediatedResponse<HttpParameterListViewModel>>
    {
        public async Task<MediatedResponse<HttpParameterListViewModel>> Handle(ListParametersQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var parameters = await repository.ListParametersAsync(query.Page);

            var viewModel = new HttpParameterListViewModel(parameters);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().HttpParameters.Count() / PwnInfraContext.Config.Api.BatchSize;

            return MediatedResponse<HttpParameterListViewModel>.Success(viewModel);
        }
    }
}