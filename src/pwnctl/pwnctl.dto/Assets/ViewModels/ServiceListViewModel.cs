namespace pwnctl.dto.Assets.ViewModels;

using pwnwrk.domain.Assets.Entities;

public sealed class ServiceListViewModel
{
    public List<Service> Services { get; init; }

    public ServiceListViewModel(List<Service> services)
    {
        Services = services;
    }
}