namespace pwnctl.dto.Assets.ViewModels;

using pwnwrk.domain.Assets.Entities;

public sealed class EndpointListViewModel
{
    public List<Endpoint> Endpoints { get; init; }

    public EndpointListViewModel(List<Endpoint> endpoints)
    {
        Endpoints = endpoints;
    }
}