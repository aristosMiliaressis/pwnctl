using pwnctl.dto.Targets.Queries;
using pwnctl.dto.Targets.ViewModels;
using pwnctl.dto.Mediator;
using pwnwrk.infra.Persistence;

using MediatR;
using pwnwrk.infra.Repositories;
using pwnwrk.domain.Assets.BaseClasses;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.ViewModels;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListNetRangesQueryHandler : IRequestHandler<ListNetRangesQuery, MediatedResponse<NetRangeListViewModel>>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse<NetRangeListViewModel>> Handle(ListNetRangesQuery command, CancellationToken cancellationToken)
        {
            AssetRepository repository = new();

            var netRanges = await repository.ListNetRangesAsync();

            return MediatedResponse<NetRangeListViewModel>.Success(new NetRangeListViewModel(netRanges));
        }
    }
}