namespace pwnctl.dto.Process.Commands;

using pwnctl.dto.Mediator;

using MediatR;

public sealed class QueryCommand : IMediatedRequest<string>
{
    public static string Route => "/query";
    public static HttpMethod Verb => HttpMethod.Post;

    public string Sql { get; set; }
}