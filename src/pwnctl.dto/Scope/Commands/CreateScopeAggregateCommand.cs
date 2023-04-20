namespace pwnctl.dto.Scope.Commands;

using pwnctl.dto.Mediator;
using pwnctl.dto.Scope.Models;

public sealed class CreateScopeAggregateCommand : ScopeRequestModel, MediatedRequest
{
    public static string Route => "/scope";
    public static HttpMethod Verb => HttpMethod.Post;
}
