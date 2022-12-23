namespace pwnctl.dto.Assets.Commands;

using pwnctl.app.Tasks.DTO;
using pwnctl.app.Tasks.Entities;
using pwnctl.dto.Mediator;

public sealed class ProcessAssetsCommand : MediatedRequest<List<TaskDTO>>
{
    public static string Route => "/assets";
    public static HttpMethod Verb => HttpMethod.Post;

    public List<string> Assets { get; set; }
}