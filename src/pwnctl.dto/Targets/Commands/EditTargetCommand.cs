namespace pwnctl.dto.Targets.Commands;

using pwnctl.app.Scope.Entities;
using pwnctl.dto.Mediator;

public sealed class EditTargetCommand : Program, MediatedRequest
{
    public static string Route => "/targets/{target}";
    public static HttpMethod Verb => HttpMethod.Put;

    public string Target { get; set; }
}