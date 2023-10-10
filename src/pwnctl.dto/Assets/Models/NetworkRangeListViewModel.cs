namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class NetworkRangeListViewModel : PaginatedViewModel<AssetDTO>
{
    public NetworkRangeListViewModel() {}

    public NetworkRangeListViewModel(IEnumerable<AssetRecord> netRanges)
    {
        Rows = netRanges.Select(e => new AssetDTO(e)).ToList();
    }
}