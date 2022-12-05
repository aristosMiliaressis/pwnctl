namespace pwnctl.dto.Db.Commands;

using pwnctl.dto.Mediator;

public sealed class SeedDbCommand : MediatedRequest
{
    public static string Route => "/db/seed";
    public static HttpMethod Verb => HttpMethod.Post;
}