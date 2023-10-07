namespace pwnctl.dto.Tasks.Commands;

using pwnctl.dto.Mediator;

public sealed class UpdateTaskProfileCommand : MediatedRequest
{
    public static string Route => "/tasks/profiles/{Name}";
    public static HttpMethod Verb => HttpMethod.Put;

    public string Name { get; set; }
}