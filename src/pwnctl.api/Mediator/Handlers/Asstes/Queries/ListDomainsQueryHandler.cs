using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.app.Common;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.Models;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListDomainsQueryHandler : IRequestHandler<ListDomainsQuery, MediatedResponse<DomainListViewModel>>
    {
        public async Task<MediatedResponse<DomainListViewModel>> Handle(ListDomainsQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var domains = await repository.ListDomainsAsync(query.Page);

            var viewModel = new DomainListViewModel(domains);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().DomainNames.Count() / Constants.BATCH_SIZE;

            return MediatedResponse<DomainListViewModel>.Success(viewModel);
        }
    }
}