namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class NetworkSocketListViewModel : PaginatedViewModel<AssetDTO>
{
    public NetworkSocketListViewModel() { }

    public NetworkSocketListViewModel(IEnumerable<AssetRecord> services)
    {
        Rows = services.Select(e => new AssetDTO(e)).ToList();
    }
}