namespace pwnctl.domain.ValueObjects;

using pwnctl.kernel.BaseClasses;

public sealed class SocketAddress : ValueObject
{
    public IpAddress IpAddress { get; }
    public ushort Port { get; }

    private SocketAddress(string address)
    {
        if (address == null || !address.Contains(":"))
            throw new ArgumentException("Invalid Socket Address: " + address, nameof(address));

        var parts = address.Split(":");

        if (!ushort.TryParse(parts[1], out ushort port))
            throw new ArgumentException("Invalid Socket Address: " + address, nameof(address));

        IpAddress = IpAddress.Create(parts[0]);
        Port = port;
    }

    public static SocketAddress Create(string address)
    {
        return new SocketAddress(address);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IpAddress;
        yield return Port;
    }
}
