namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Targets.Entities;
using pwnctl.dto.Mediator;

using MediatR;

public sealed class OnboardTargetCommand : Program, IMediatedRequest
{
    public static string Route => "/targets";
    public static HttpMethod Verb => HttpMethod.Post;
}