namespace pwnctl.dto.Operations.Commands;

using pwnctl.dto.Mediator;

public sealed class ResumeOperationCommand : MediatedRequest
{
    public static string Route => "/ops/{Name}/resume";
    public static HttpMethod Verb => HttpMethod.Post;

    public string Name { get; set; }
}