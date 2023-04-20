namespace pwnctl.dto.Tasks.Commands;

using pwnctl.dto.Mediator;

public sealed class UpdateTaskDefinitionCommand : MediatedRequest
{
    public static string Route => "/tasks/definitions/{ShortName}";
    public static HttpMethod Verb => HttpMethod.Put;

    public string ShortName { get; set; }
}