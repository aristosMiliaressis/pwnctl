namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;

using MediatR;

public class CreateTargetCommand : Program, IApiRequest<object>, IRequest<MediatorResult>
{
    public static string Route => "/targets";
    public static HttpMethod Method => HttpMethod.Post;
}