namespace pwnctl.dto.Process.Commands;

using pwnwrk.domain.Entities;
using pwnctl.dto.Mediator;

using MediatR;

public class QueryCommand : IMediatedRequest<string>
{
    public static string Route => "/query";
    public static HttpMethod Method => HttpMethod.Post;

    public string Sql { get; set; }
}