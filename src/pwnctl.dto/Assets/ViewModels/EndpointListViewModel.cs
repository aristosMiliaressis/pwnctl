namespace pwnctl.dto.Assets.ViewModels;

using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;

public sealed class EndpointListViewModel
{
    public IEnumerable<AssetDTO> Endpoints { get; init; }

    public EndpointListViewModel() { }

    public EndpointListViewModel(List<AssetRecord> endpoints)
    {
        Endpoints = endpoints.Select(ep => ep.ToDTO());
    }
}