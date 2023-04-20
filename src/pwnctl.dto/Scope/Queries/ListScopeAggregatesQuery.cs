namespace pwnctl.dto.Scope.Queries;

using pwnctl.dto.Mediator;
using pwnctl.dto.Scope.Models;

public sealed class ListScopeAggregatesQuery : MediatedRequest<ScopeListViewModel>
{
    public static string Route => "/scope";
    public static HttpMethod Verb => HttpMethod.Get;
}