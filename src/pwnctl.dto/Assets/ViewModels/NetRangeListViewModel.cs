namespace pwnctl.dto.Assets.ViewModels;

using pwnctl.domain.Entities;

public sealed class NetRangeListViewModel
{
    public List<NetRange> NetRanges { get; init; }

    public NetRangeListViewModel(List<NetRange> netRanges)
    {
        NetRanges = netRanges;
    }
}