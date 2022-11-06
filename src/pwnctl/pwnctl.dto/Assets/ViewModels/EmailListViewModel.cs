namespace pwnctl.dto.Assets.ViewModels;

using pwnwrk.domain.Assets.Entities;

public sealed class EmailListViewModel
{
    public List<Email> Emails { get; init; }

    public EmailListViewModel(List<Email> emails)
    {
        Emails = emails;
    }
}