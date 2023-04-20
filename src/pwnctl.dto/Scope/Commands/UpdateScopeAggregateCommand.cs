namespace pwnctl.dto.Scope.Commands;

using pwnctl.dto.Mediator;

public sealed class UpdateScopeAggregateCommand : MediatedRequest
{
    public static string Route => "/scope/{id}";
    public static HttpMethod Verb => HttpMethod.Put;
}