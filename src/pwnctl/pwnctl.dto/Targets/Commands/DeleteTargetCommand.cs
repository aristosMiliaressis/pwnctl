namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Targets.Entities;
using pwnctl.dto.Mediator;

using MediatR;

public class DeleteTargetCommand : Program, IMediatedRequest
{
    public static string Route => "/targets/{target}";
    public static HttpMethod Method => HttpMethod.Delete;

    public string Target { get; set; }
}