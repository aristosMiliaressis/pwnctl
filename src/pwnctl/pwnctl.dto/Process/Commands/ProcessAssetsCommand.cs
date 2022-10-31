namespace pwnctl.dto.Process.Commands;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;

using MediatR;

public class ProcessAssetsCommand : ScopeDefinition, IApiRequest<object>, IRequest<MediatorResult>
{
    public static string Route => "/process";
    public static HttpMethod Method => HttpMethod.Post;

    public List<string> Assets { get; set; }
}