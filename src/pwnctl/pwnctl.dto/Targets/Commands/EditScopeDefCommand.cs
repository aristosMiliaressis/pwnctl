namespace pwnctl.dto.Targets.Commands;

using MediatR;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;

public class EditScopeDefCommand : ScopeDefinition, IApiRequest<object>, IRequest<MediatorResult>
{
    public static string Route => "/targets/{target}/scope/{scope}";
    public static HttpMethod Method => HttpMethod.Patch;

    public string Target { get; set; }
}