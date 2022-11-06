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
    public sealed class ListDomainsQueryHandler : IRequestHandler<ListDomainsQuery, MediatedResponse<DomainListViewModel>>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse<DomainListViewModel>> Handle(ListDomainsQuery command, CancellationToken cancellationToken)
        {
            AssetRepository repository = new();

            var domains = await repository.ListDomainsAsync();

            return MediatedResponse<DomainListViewModel>.Success(new DomainListViewModel(domains));
        }
    }
}