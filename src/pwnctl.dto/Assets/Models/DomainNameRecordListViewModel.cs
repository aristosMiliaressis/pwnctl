namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class DomainNameRecordListViewModel : PaginatedViewModel<AssetDTO>
{
    public DomainNameRecordListViewModel() { }

    public DomainNameRecordListViewModel(IEnumerable<AssetRecord> dnsRecords)
    {
        Rows = dnsRecords.Select(e => new AssetDTO(e)).ToList();
    }
}