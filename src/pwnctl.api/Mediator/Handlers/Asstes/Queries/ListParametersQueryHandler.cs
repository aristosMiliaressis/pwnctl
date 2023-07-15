using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.app.Common;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.Models;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListParametersQueryHandler : IRequestHandler<ListParametersQuery, MediatedResponse<ParamListViewModel>>
    {
        public async Task<MediatedResponse<ParamListViewModel>> Handle(ListParametersQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var parameters = await repository.ListParametersAsync(query.Page);

            var viewModel = new ParamListViewModel(parameters);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().HttpParameters.Count() / Constants.BATCH_SIZE;

            return MediatedResponse<ParamListViewModel>.Success(viewModel);
        }
    }
}