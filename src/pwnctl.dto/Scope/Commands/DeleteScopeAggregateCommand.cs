namespace pwnctl.dto.Scope.Commands;

using pwnctl.dto.Mediator;

public sealed class DeleteScopeAggregateCommand : MediatedRequest
{
    public static string Route => "/scope/{Name}";
    public static HttpMethod Verb => HttpMethod.Delete;

    public string Name { get; set; }
}