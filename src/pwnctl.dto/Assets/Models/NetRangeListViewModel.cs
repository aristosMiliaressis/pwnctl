namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class NetRangeListViewModel : PaginatedViewModel<AssetDTO>
{
    public NetRangeListViewModel() {}

    public NetRangeListViewModel(List<AssetRecord> netRanges)
    {
        Rows = netRanges.Select(e => new AssetDTO(e)).ToList();
    }
}