namespace pwnctl.dto.Operations.Queries;

using pwnctl.dto.Operations.Models;
using pwnctl.dto.Mediator;

public sealed class ListOperationsQuery : MediatedRequest<OperationListViewModel>
{
    public static string Route => "/targets";
    public static HttpMethod Verb => HttpMethod.Get;
}