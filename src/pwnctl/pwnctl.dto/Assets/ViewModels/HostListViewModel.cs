namespace pwnctl.dto.Assets.ViewModels;

using pwnwrk.domain.Assets.Entities;

public sealed class HostListViewModel
{
    public List<Host> Hosts { get; init; }

    public HostListViewModel(List<Host> hosts)
    {
        Hosts = hosts;
    }
}