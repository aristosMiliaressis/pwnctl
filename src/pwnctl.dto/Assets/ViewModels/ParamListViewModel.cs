namespace pwnctl.dto.Assets.ViewModels;

using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;

public sealed class ParamListViewModel
{
    public IEnumerable<AssetDTO> Parameters { get; init; }

    public ParamListViewModel() { }

    public ParamListViewModel(List<AssetRecord> parameters)
    {
        Parameters = parameters.Select(e => new AssetDTO(e));
    }
}