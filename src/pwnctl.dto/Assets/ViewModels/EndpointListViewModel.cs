namespace pwnctl.dto.Assets.ViewModels;

using pwnctl.domain.Entities;

public sealed class EndpointListViewModel
{
    public List<Endpoint> Endpoints { get; init; }

    public EndpointListViewModel(List<Endpoint> endpoints)
    {
        Endpoints = endpoints;
    }
}