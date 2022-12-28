namespace pwnctl.dto.Assets.ViewModels;

using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;
using pwnctl.domain.Entities;

public sealed class DomainListViewModel
{
    public IEnumerable<AssetDTO> Domains { get; init; }

    public DomainListViewModel(List<AssetRecord> domains)
    {
        Domains = domains.Select(e => e.ToDTO());
    }
}