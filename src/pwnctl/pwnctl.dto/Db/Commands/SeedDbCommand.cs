namespace pwnctl.dto.Db.Commands;

using pwnctl.dto.Mediator;

public sealed class SeedDbCommand : IMediatedRequest
{
    public static string Route => "/db/seed";
    public static HttpMethod Verb => HttpMethod.Post;
}