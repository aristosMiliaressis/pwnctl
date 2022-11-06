namespace pwnctl.dto.Assets.Commands;

using pwnctl.dto.Mediator;

using MediatR;

public sealed class ProcessAssetsCommand : IMediatedRequest
{
    public static string Route => "/assets";
    public static HttpMethod Verb => HttpMethod.Post;

    public List<string> Assets { get; set; }
}