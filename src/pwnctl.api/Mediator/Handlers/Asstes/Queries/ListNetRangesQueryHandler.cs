using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.ViewModels;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListNetRangesQueryHandler : IRequestHandler<ListNetRangesQuery, MediatedResponse<NetRangeListViewModel>>
    {
        public async Task<MediatedResponse<NetRangeListViewModel>> Handle(ListNetRangesQuery command, CancellationToken cancellationToken)
        {
            PwnctlDbContext context = new();
            AssetDbRepository repository = new(context);

            var netRanges = await repository.ListNetRangesAsync();

            return MediatedResponse<NetRangeListViewModel>.Success(new NetRangeListViewModel(netRanges));
        }
    }
}