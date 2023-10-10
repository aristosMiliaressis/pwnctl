namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class HttpParameterListViewModel : PaginatedViewModel<AssetDTO>
{
    public HttpParameterListViewModel() { }

    public HttpParameterListViewModel(IEnumerable<AssetRecord> parameters)
    {
        Rows = parameters.Select(e => new AssetDTO(e)).ToList();
    }
}