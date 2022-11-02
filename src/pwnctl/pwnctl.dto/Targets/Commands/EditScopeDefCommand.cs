namespace pwnctl.dto.Targets.Commands;

using MediatR;

using pwnwrk.domain.Targets.Entities;
using pwnctl.dto.Mediator;

public class EditScopeDefCommand : ScopeDefinition, IMediatedRequest
{
    public static string Route => "/targets/{target}/scope/{scope}";
    public static HttpMethod Method => HttpMethod.Patch;

    public string Target { get; set; }
}