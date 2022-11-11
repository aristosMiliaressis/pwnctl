using pwnctl.dto.Mediator;
using pwnwrk.infra.Persistence;

using MediatR;
using pwnwrk.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.ViewModels;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListDnsRecordsQueryHandler : IRequestHandler<ListDnsRecordsQuery, MediatedResponse<DnsRecordListViewModel>>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse<DnsRecordListViewModel>> Handle(ListDnsRecordsQuery command, CancellationToken cancellationToken)
        {
            AssetRepository repository = new();

            var records = await repository.ListDNSRecordsAsync();

            return MediatedResponse<DnsRecordListViewModel>.Success(new DnsRecordListViewModel(records));
        }
    }
}