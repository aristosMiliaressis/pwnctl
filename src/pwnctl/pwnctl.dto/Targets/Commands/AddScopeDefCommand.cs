namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Entities;
using pwnctl.dto.Mediator;

using MediatR;

public class AddScopeDefCommand : ScopeDefinition, IMediatedRequest
{
    public static string Route => "/targets/{target}/scope";
    public static HttpMethod Method => HttpMethod.Post;

    public string Target { get; set; }
}