namespace pwnctl.dto.Targets.Commands;

using pwnctl.app.Entities;
using pwnctl.dto.Mediator;

public sealed class EditScopeDefCommand : ScopeDefinition, MediatedRequest
{
    public static string Route => "/targets/{target}/scope/{scope}";
    public static HttpMethod Verb => HttpMethod.Put;

    public string Target { get; set; }
}