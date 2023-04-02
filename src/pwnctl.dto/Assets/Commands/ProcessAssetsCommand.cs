namespace pwnctl.dto.Assets.Commands;

using pwnctl.app.Queueing.DTO;
using pwnctl.dto.Mediator;

public sealed class ProcessAssetsCommand : MediatedRequest<List<PendingTaskDTO>>
{
    public static string Route => "/assets";
    public static HttpMethod Verb => HttpMethod.Post;

    public List<string> Assets { get; set; }
}