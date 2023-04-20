namespace pwnctl.dto.Tasks.Commands;

using pwnctl.dto.Mediator;

public sealed class DeleteTaskProfileCommand : MediatedRequest
{
    public static string Route => "/tasks/profiles/{ShortName}";
    public static HttpMethod Verb => HttpMethod.Delete;

    public string ShortName { get; set; }
}