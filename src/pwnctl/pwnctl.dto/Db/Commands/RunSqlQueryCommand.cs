namespace pwnctl.dto.Db.Commands;

using pwnctl.dto.Mediator;

public sealed class RunSqlQueryCommand : IMediatedRequest<List<object>>
{
    public static string Route => "/db/query";
    public static HttpMethod Verb => HttpMethod.Post;

    public string Query { get; set; }
}