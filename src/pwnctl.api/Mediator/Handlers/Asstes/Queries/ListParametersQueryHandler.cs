using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.ViewModels;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListParametersQueryHandler : IRequestHandler<ListParametersQuery, MediatedResponse<ParamListViewModel>>
    {
        public async Task<MediatedResponse<ParamListViewModel>> Handle(ListParametersQuery command, CancellationToken cancellationToken)
        {
            PwnctlDbContext context = new();
            AssetDbRepository repository = new(context);

            var parameters = await repository.ListParametersAsync();

            return MediatedResponse<ParamListViewModel>.Success(new ParamListViewModel(parameters));
        }
    }
}