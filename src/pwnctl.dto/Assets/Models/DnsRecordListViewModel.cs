namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class DnsRecordListViewModel : PaginatedViewModel<AssetDTO>
{
    public DnsRecordListViewModel() { }

    public DnsRecordListViewModel(List<AssetRecord> dnsRecords)
    {
        Rows = dnsRecords.Select(e => new AssetDTO(e)).ToList();
    }
}