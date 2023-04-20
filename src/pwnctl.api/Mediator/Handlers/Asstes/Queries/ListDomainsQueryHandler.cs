using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.Models;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListDomainsQueryHandler : IRequestHandler<ListDomainsQuery, MediatedResponse<DomainListViewModel>>
    {
        public async Task<MediatedResponse<DomainListViewModel>> Handle(ListDomainsQuery command, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var domains = await repository.ListDomainsAsync();

            return MediatedResponse<DomainListViewModel>.Success(new DomainListViewModel(domains));
        }
    }
}