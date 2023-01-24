using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.ViewModels;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListHostsQueryHandler : IRequestHandler<ListHostsQuery, MediatedResponse<HostListViewModel>>
    {
        public async Task<MediatedResponse<HostListViewModel>> Handle(ListHostsQuery command, CancellationToken cancellationToken)
        {
            PwnctlDbContext context = new();
            AssetDbRepository repository = new(context);

            var hosts = await repository.ListHostsAsync();

            return MediatedResponse<HostListViewModel>.Success(new HostListViewModel(hosts));
        }
    }
}