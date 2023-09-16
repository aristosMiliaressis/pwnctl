namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class DomainListViewModel : PaginatedViewModel<AssetDTO>
{
    public DomainListViewModel() {}

    public DomainListViewModel(List<AssetRecord> domains)
    {
        Rows = domains.Select(e => new AssetDTO(e)).ToList();
    }
}