namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class DomainNameListViewModel : PaginatedViewModel<AssetDTO>
{
    public DomainNameListViewModel() {}

    public DomainNameListViewModel(IEnumerable<AssetRecord> domains)
    {
        Rows = domains.Select(e => new AssetDTO(e)).ToList();
    }
}