using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.Models;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListParametersQueryHandler : IRequestHandler<ListParametersQuery, MediatedResponse<ParamListViewModel>>
    {
        public async Task<MediatedResponse<ParamListViewModel>> Handle(ListParametersQuery command, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var parameters = await repository.ListParametersAsync();

            return MediatedResponse<ParamListViewModel>.Success(new ParamListViewModel(parameters));
        }
    }
}