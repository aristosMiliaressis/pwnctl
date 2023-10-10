namespace pwnctl.dto.Assets.Models;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Mediator;

public sealed class EmailListViewModel : PaginatedViewModel<AssetDTO>
{
    public EmailListViewModel() { }

    public EmailListViewModel(IEnumerable<AssetRecord> emails)
    {
        Rows = emails.Select(e => new AssetDTO(e)).ToList();
    }
}