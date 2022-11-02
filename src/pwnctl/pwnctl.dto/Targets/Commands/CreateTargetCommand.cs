namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Targets.Entities;
using pwnctl.dto.Mediator;

using MediatR;

public class CreateTargetCommand : Program, IMediatedRequest
{
    public static string Route => "/targets";
    public static HttpMethod Method => HttpMethod.Post;
}