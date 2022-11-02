namespace pwnctl.dto.Process.Commands;

using pwnctl.dto.Mediator;

using MediatR;

public class ProcessAssetsCommand : IMediatedRequest
{
    public static string Route => "/assets";
    public static HttpMethod Method => HttpMethod.Post;

    public List<string> Assets { get; set; }
}