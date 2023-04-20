namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;

public sealed class NetRangeListViewModel
{
    public IEnumerable<AssetDTO> NetRanges { get; init; }

    public NetRangeListViewModel() {}

    public NetRangeListViewModel(List<AssetRecord> netRanges)
    {
        NetRanges = netRanges.Select(e => new AssetDTO(e));
    }
}