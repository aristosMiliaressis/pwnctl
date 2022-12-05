namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Targets.Entities;
using pwnctl.dto.Mediator;

public sealed class AddScopeDefCommand : ScopeDefinition, MediatedRequest
{
    public static string Route => "/targets/{target}/scope";
    public static HttpMethod Verb => HttpMethod.Post;

    public string Target { get; set; }
}