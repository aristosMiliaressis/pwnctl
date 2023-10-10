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
    public sealed class ListDomainsQueryHandler : IRequestHandler<ListDomainsQuery, MediatedResponse<DomainNameListViewModel>>
    {
        public async Task<MediatedResponse<DomainNameListViewModel>> Handle(ListDomainsQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var domains = await repository.ListDomainsAsync(query.Page);

            var viewModel = new DomainNameListViewModel(domains);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().DomainNames.Count() / PwnInfraContext.Config.Api.BatchSize;

            return MediatedResponse<DomainNameListViewModel>.Success(viewModel);
        }
    }
}