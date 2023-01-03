namespace pwnctl.dto.Assets.ViewModels;

using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;

public sealed class DnsRecordListViewModel
{
    public IEnumerable<AssetDTO> DNSRecords { get; init; }

    public DnsRecordListViewModel() { }

    public DnsRecordListViewModel(List<AssetRecord> dnsRecords)
    {
        DNSRecords = dnsRecords.Select(e => new AssetDTO(e));
    }
}