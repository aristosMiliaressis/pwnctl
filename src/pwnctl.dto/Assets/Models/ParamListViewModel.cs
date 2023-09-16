namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class ParamListViewModel : PaginatedViewModel<AssetDTO>
{
    public ParamListViewModel() { }

    public ParamListViewModel(List<AssetRecord> parameters)
    {
        Rows = parameters.Select(e => new AssetDTO(e)).ToList();
    }
}