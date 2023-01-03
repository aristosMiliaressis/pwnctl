namespace pwnctl.dto.Assets.ViewModels;

using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;

public sealed class HostListViewModel
{
    public IEnumerable<AssetDTO> Hosts { get; init; }

    public HostListViewModel() { }

    public HostListViewModel(List<AssetRecord> hosts)
    {
        Hosts = hosts.Select(e => new AssetDTO(e));
    }
}