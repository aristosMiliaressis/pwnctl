namespace pwnctl.dto.Operations.Commands;

using pwnctl.app.Operations.Enums;
using pwnctl.dto.Mediator;

public sealed class UpdateOperationCommand : MediatedRequest
{
    public static string Route => "/ops/{Name}";
    public static HttpMethod Verb => HttpMethod.Put;

    public string Name { get; set; }
    public OperationState State { get; set; }
}