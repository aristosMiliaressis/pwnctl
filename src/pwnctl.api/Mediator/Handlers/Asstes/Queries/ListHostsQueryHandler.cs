using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.app.Common;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.Models;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListHostsQueryHandler : IRequestHandler<ListHostsQuery, MediatedResponse<HostListViewModel>>
    {
        public async Task<MediatedResponse<HostListViewModel>> Handle(ListHostsQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var hosts = await repository.ListHostsAsync(query.Page);

            var viewModel = new HostListViewModel(hosts);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().HttpHosts.Count() / Constants.BATCH_SIZE;

            return MediatedResponse<HostListViewModel>.Success(viewModel);
        }
    }
}