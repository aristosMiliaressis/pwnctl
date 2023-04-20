namespace pwnctl.dto.Operations.Commands;

using pwnctl.dto.Mediator;

public sealed class DeleteOperationCommand : MediatedRequest
{
    public static string Route => "/ops/{ShortName}";
    public static HttpMethod Verb => HttpMethod.Delete;

    public string ShortName { get; set; }
}