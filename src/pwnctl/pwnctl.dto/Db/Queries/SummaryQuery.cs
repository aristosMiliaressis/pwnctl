namespace pwnctl.dto.Db.Queries;

using pwnctl.dto.Mediator;
using pwnctl.dto.Db.ViewModels;

using MediatR;

public sealed class SummaryQuery : IMediatedRequest<SummaryViewModel>
{
    public static string Route => "/db/summary";
    public static HttpMethod Verb => HttpMethod.Get;
}