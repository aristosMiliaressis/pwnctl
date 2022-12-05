namespace pwnctl.dto.Db.Queries;

using pwnctl.dto.Mediator;
using pwnctl.dto.Db.ViewModels;

public sealed class SummaryQuery : MediatedRequest<SummaryViewModel>
{
    public static string Route => "/db/summary";
    public static HttpMethod Verb => HttpMethod.Get;
}