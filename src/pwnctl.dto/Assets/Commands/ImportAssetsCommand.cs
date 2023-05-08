namespace pwnctl.dto.Assets.Commands;

using pwnctl.dto.Mediator;

public sealed class ImportAssetsCommand : MediatedRequest
{
    public static string Route => "/assets";
    public static HttpMethod Verb => HttpMethod.Post;

    public List<string> Assets { get; set; }
}
