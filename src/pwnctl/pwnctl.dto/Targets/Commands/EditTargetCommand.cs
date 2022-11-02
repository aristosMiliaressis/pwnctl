namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Targets.Entities;
using pwnctl.dto.Mediator;

using MediatR;

public sealed class EditTargetCommand : Program, IMediatedRequest
{
    public static string Route => "/targets/{target}";
    public static HttpMethod Method => HttpMethod.Patch;

    public string Target { get; set; }
}