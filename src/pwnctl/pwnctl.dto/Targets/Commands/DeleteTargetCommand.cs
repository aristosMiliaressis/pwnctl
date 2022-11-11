namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Targets.Entities;
using pwnctl.dto.Mediator;

public sealed class DeleteTargetCommand : Program, IMediatedRequest
{
    public static string Route => "/targets/{target}";
    public static HttpMethod Verb => HttpMethod.Delete;

    public string Target { get; set; }
}