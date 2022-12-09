namespace pwnctl.dto.Assets.ViewModels;

using pwnctl.domain.Entities;

public sealed class DomainListViewModel
{
    public List<Domain> Domains { get; init; }

    public DomainListViewModel() {}

    public DomainListViewModel(List<Domain> domains)
    {
        Domains = domains;
    }
}