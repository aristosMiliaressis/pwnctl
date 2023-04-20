namespace pwnctl.dto.Tasks.Commands;

using pwnctl.dto.Mediator;

public sealed class UpdateTaskProfileCommand : MediatedRequest
{
    public static string Route => "/tasks/profiles/{ShortName}";
    public static HttpMethod Verb => HttpMethod.Put;

    public string ShortName { get; set; }
}