namespace pwnctl.dto.Assets.Commands;

using pwnctl.app.Queueing.DTO;
using pwnctl.app.Tasks.DTO;
using pwnctl.app.Tasks.Entities;
using pwnctl.dto.Mediator;

public sealed class ProcessAssetsCommand : MediatedRequest<List<QueuedTaskDTO>>
{
    public static string Route => "/assets";
    public static HttpMethod Verb => HttpMethod.Post;

    public List<string> Assets { get; set; }
}