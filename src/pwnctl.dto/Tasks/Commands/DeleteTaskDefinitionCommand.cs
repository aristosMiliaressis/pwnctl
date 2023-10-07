namespace pwnctl.dto.Tasks.Commands;

using pwnctl.dto.Mediator;

public sealed class DeleteTaskDefinitionCommand : MediatedRequest
{
    public static string Route => "/tasks/definitions/{Name}";
    public static HttpMethod Verb => HttpMethod.Delete;
    
    public string Name { get; set; }
}