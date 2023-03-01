using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.ViewModels;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListDnsRecordsQueryHandler : IRequestHandler<ListDnsRecordsQuery, MediatedResponse<DnsRecordListViewModel>>
    {
        public async Task<MediatedResponse<DnsRecordListViewModel>> Handle(ListDnsRecordsQuery command, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var records = await repository.ListDNSRecordsAsync();

            return MediatedResponse<DnsRecordListViewModel>.Success(new DnsRecordListViewModel(records));
        }
    }
}