namespace pwnctl.dto.Assets.ViewModels;

using pwnwrk.domain.Assets.Entities;

public sealed class DomainListViewModel
{
    public List<Domain> Domains { get; init; }

    public DomainListViewModel(List<Domain> domains)
    {
        Domains = domains;
    }
}