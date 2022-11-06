namespace pwnctl.dto.Targets.Commands;

using MediatR;

using pwnwrk.domain.Targets.Entities;
using pwnctl.dto.Mediator;

public sealed class EditScopeDefCommand : ScopeDefinition, IMediatedRequest
{
    public static string Route => "/targets/{target}/scope/{scope}";
    public static HttpMethod Verb => HttpMethod.Put;

    public string Target { get; set; }
}