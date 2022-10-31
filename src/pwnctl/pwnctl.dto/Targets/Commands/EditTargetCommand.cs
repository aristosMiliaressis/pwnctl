namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;

using MediatR;

public class EditTargetCommand : Program, IApiRequest<object>, IRequest<MediatorResult>
{
    public static string Route => "/targets/{target}";
    public static HttpMethod Method => HttpMethod.Patch;


}