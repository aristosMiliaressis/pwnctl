namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Targets.Entities;
using pwnctl.dto.Mediator;

public sealed class OnboardTargetCommand : Program, MediatedRequest
{
    public static string Route => "/targets";
    public static HttpMethod Verb => HttpMethod.Post;
}