namespace pwnctl.dto.Assets.ViewModels;

using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;

public sealed class HostListViewModel
{
    public IEnumerable<AssetDTO> Hosts { get; init; }

    public HostListViewModel(List<AssetRecord> hosts)
    {
        Hosts = hosts.Select(e => e.ToDTO());
    }
}