namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class EndpointListViewModel : PaginatedViewModel<AssetDTO>
{
    public EndpointListViewModel() { }

    public EndpointListViewModel(List<AssetRecord> endpoints)
    {
        Rows = endpoints.Select(e => new AssetDTO(e)).ToList();
    }
}