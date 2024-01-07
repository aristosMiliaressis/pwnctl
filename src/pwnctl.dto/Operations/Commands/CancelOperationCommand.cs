namespace pwnctl.dto.Operations.Commands;

using pwnctl.dto.Mediator;

public sealed class CancelOperationCommand : MediatedRequest
{
    public static string Route => "/ops/{Name}/cancel";
    public static HttpMethod Verb => HttpMethod.Post;

    public string Name { get; set; }
}