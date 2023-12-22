namespace pwnctl.dto.Operations.Queries;

using pwnctl.dto.Mediator;
using pwnctl.dto.Operations.Models;

public sealed class OperationSummaryQuery : MediatedRequest<SummaryViewModel>
{
    public static string Route => "/ops/{Name}";
    public static HttpMethod Verb => HttpMethod.Get;

    public string Name { get; set; }
}