namespace pwnctl.dto.Assets.Commands;

using pwnctl.app.Tasks.Entities;
using pwnctl.dto.Mediator;

public sealed class ProcessAssetsCommand : MediatedRequest<List<TaskRecord>>
{
    public static string Route => "/assets";
    public static HttpMethod Verb => HttpMethod.Post;

    public List<string> Assets { get; set; }
}