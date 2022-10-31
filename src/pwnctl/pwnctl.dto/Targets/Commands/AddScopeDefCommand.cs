namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;

using MediatR;

public class AddScopeDefCommand : ScopeDefinition, IApiRequest<object>, IRequest<MediatorResult>
{
    public static string Route => "/targets/{target}/scope";
    public static HttpMethod Method => HttpMethod.Post;

    public string Target { get; set; }
}