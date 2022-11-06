namespace pwnctl.dto.Assets.ViewModels;

using pwnwrk.domain.Assets.Entities;

public sealed class DnsRecordListViewModel
{
    public List<DNSRecord> DNSRecords { get; init; }

    public DnsRecordListViewModel(List<DNSRecord> dnsRecords)
    {
        DNSRecords = dnsRecords;
    }
}