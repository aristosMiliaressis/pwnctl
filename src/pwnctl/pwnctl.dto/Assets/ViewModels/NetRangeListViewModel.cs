namespace pwnctl.dto.Assets.ViewModels;

using pwnwrk.domain.Assets.Entities;

public sealed class NetRangeListViewModel
{
    public List<NetRange> NetRanges { get; init; }

    public NetRangeListViewModel(List<NetRange> netRanges)
    {
        NetRanges = netRanges;
    }
}