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
    public sealed class ListVirtualHostsQueryHandler : IRequestHandler<ListVirtualHostsQuery, MediatedResponse<VirtualHostListViewModel>>
    {
        public async Task<MediatedResponse<VirtualHostListViewModel>> Handle(ListVirtualHostsQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var emails = await repository.ListVirtualHostsAsync(query.Page);

            var viewModel = new VirtualHostListViewModel(emails);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().VirtualHosts.Count() / PwnInfraContext.Config.Api.BatchSize;

            return MediatedResponse<VirtualHostListViewModel>.Success(viewModel);
        }
    }
}