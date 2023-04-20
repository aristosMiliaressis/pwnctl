namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;

public sealed class ServiceListViewModel
{
    public IEnumerable<AssetDTO> Services { get; init; }

    public ServiceListViewModel() { }

    public ServiceListViewModel(List<AssetRecord> services)
    {
        Services = services.Select(e => new AssetDTO(e));
    }
}