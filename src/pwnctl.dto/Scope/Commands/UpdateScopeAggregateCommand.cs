namespace pwnctl.dto.Scope.Commands;

using pwnctl.dto.Mediator;

public sealed class UpdateScopeAggregateCommand : MediatedRequest
{
    public static string Route => "/scope/{ShortName}";
    public static HttpMethod Verb => HttpMethod.Put;

    public string ShortName { get; set; }
}
