namespace pwnctl.dto.Db.Queries;

using pwnctl.dto.Mediator;
using pwnctl.dto.Db.ViewModels;

public sealed class ExportQuery : IMediatedRequest<ExportViewModel>
{
    public static string Route => "/db/export";
    public static HttpMethod Verb => HttpMethod.Get;
}