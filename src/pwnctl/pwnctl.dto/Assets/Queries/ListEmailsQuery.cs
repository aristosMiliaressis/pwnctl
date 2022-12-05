namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.ViewModels;
using pwnctl.dto.Mediator;

public sealed class ListEmailsQuery : MediatedRequest<EmailListViewModel>
{
    public static string Route => "/assets/emails";
    public static HttpMethod Verb => HttpMethod.Get;
}