namespace pwnctl.dto.Process.Commands;

using pwnwrk.domain.Entities;
using pwnctl.dto.Mediator;

using MediatR;

public class ProcessAssetsCommand : ScopeDefinition, IMediatedRequest
{
    public static string Route => "/assets";
    public static HttpMethod Method => HttpMethod.Post;

    public List<string> Assets { get; set; }
}