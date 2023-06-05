using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.Models;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListDnsRecordsQueryHandler : IRequestHandler<ListDnsRecordsQuery, MediatedResponse<DnsRecordListViewModel>>
    {
        public async Task<MediatedResponse<DnsRecordListViewModel>> Handle(ListDnsRecordsQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var records = await repository.ListDNSRecordsAsync(query.Page);

            var viewModel = new DnsRecordListViewModel(records);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().DomainNameRecords.Count() / 4096;

            return MediatedResponse<DnsRecordListViewModel>.Success(viewModel);
        }
    }
}