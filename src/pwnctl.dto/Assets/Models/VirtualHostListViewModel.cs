namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class VirtualHostListViewModel : PaginatedViewModel<AssetDTO>
{
    public VirtualHostListViewModel() { }

    public VirtualHostListViewModel(IEnumerable<AssetRecord> emails)
    {
        Rows = emails.Select(e => new AssetDTO(e)).ToList();
    }
}