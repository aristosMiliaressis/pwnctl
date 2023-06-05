namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class ServiceListViewModel : PaginatedViewModel<AssetDTO>
{
    public ServiceListViewModel() { }

    public ServiceListViewModel(List<AssetRecord> services)
    {
        Rows = services.Select(e => new AssetDTO(e)).ToList();
    }
}