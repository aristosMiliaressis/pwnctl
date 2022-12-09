namespace pwnctl.dto.Assets.ViewModels;

using pwnctl.domain.Entities;

public sealed class DnsRecordListViewModel
{
    public List<DNSRecord> DNSRecords { get; init; }

    public DnsRecordListViewModel(List<DNSRecord> dnsRecords)
    {
        DNSRecords = dnsRecords;
    }
}