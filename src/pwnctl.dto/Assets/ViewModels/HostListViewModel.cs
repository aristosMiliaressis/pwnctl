namespace pwnctl.dto.Assets.ViewModels;

using pwnctl.domain.Entities;

public sealed class HostListViewModel
{
    public List<Host> Hosts { get; init; }

    public HostListViewModel(List<Host> hosts)
    {
        Hosts = hosts;
    }
}