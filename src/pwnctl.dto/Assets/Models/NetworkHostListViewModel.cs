namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class NetworkHostListViewModel : PaginatedViewModel<AssetDTO>
{
    public NetworkHostListViewModel() { }

    public NetworkHostListViewModel(IEnumerable<AssetRecord> hosts)
    {
        Rows = hosts.Select(e => new AssetDTO(e)).ToList();
    }
}