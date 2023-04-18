namespace pwnctl.dto.Operations.Commands;

using pwnctl.app.Operations.Entities;
using pwnctl.dto.Mediator;

public sealed class CreateOperationCommand : Operation, MediatedRequest
{
    public static string Route => "/ops";
    public static HttpMethod Verb => HttpMethod.Post;
}