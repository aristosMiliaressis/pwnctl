namespace pwnctl.dto.Assets.ViewModels;

using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;

public sealed class EmailListViewModel
{
    public IEnumerable<AssetDTO> Emails { get; init; }

    public EmailListViewModel() { }

    public EmailListViewModel(List<AssetRecord> emails)
    {
        Emails = emails.Select(e => e.ToDTO());
    }
}