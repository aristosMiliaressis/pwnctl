namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class HostListViewModel : PaginatedViewModel<AssetDTO>
{
    public HostListViewModel() { }

    public HostListViewModel(List<AssetRecord> hosts)
    {
        Rows = hosts.Select(e => new AssetDTO(e)).ToList();
    }
}