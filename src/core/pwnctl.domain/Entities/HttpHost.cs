namespace pwnctl.domain.Entities;

using pwnctl.kernel.BaseClasses;
using pwnctl.kernel.Attributes;

public sealed class HttpHost : Entity<Guid>//: Asset
{
    [EqualityComponent]
    public string Name { get; private init; }
    [EqualityComponent]
    public string SocketAddress { get; private init; }
    public NetworkSocket Socket { get; private init; }
    public Guid ServiceId { get; private init; }

    private HttpHost() {}

    public HttpHost(NetworkSocket address, string name)
    {
        Socket = address;
        SocketAddress = address.Address;
        Name = name;
    }

    public static Result<HttpHost, string> TryParse(string assetText)
    {
        try
        {
            return $"{assetText} is not a {nameof(HttpHost)}";
        }
        catch
        {
            return $"{assetText} is not a {nameof(HttpHost)}";
        }
    }

    public override string ToString()
    {
        return SocketAddress + "\t" + Name;
    }
}
