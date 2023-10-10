namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class HttpEndpointListViewModel : PaginatedViewModel<AssetDTO>
{
    public HttpEndpointListViewModel() { }

    public HttpEndpointListViewModel(IEnumerable<AssetRecord> endpoints)
    {
        Rows = endpoints.Select(e => new AssetDTO(e)).ToList();
    }
}