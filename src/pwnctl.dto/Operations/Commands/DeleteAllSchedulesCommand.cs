namespace pwnctl.dto.Operations.Commands;

using pwnctl.dto.Mediator;

public sealed class DeleteAllSchedulesCommand : MediatedRequest
{
    public static string Route => "/ops/";
    public static HttpMethod Verb => HttpMethod.Delete;
}
