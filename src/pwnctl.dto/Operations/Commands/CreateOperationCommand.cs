namespace pwnctl.dto.Operations.Commands;

using pwnctl.dto.Mediator;
using pwnctl.dto.Operations.Models;

public sealed class CreateOperationCommand : OperationRequestModel, MediatedRequest
{
    public static string Route => "/ops";
    public static HttpMethod Verb => HttpMethod.Post;
}