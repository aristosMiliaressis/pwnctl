namespace pwnctl.dto.Operations.Commands;

using pwnctl.dto.Mediator;

public sealed class PauseOperationCommand : MediatedRequest
{
    public static string Route => "/ops/{Name}/pause";
    public static HttpMethod Verb => HttpMethod.Post;

    public string Name { get; set; }
}