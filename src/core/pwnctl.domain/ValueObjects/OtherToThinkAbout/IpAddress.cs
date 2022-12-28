namespace pwnctl.domain.ValueObjects;

using pwnctl.kernel.BaseClasses;

public sealed class IpAddress : ValueObject
{
    public string Address { get; }

    private IpAddress(string address)
    {
        if (Uri.CheckHostName(address) != UriHostNameType.IPv4 
         && Uri.CheckHostName(address) != UriHostNameType.IPv6)
            throw new ArgumentException("Invalid IP address: " + address, nameof(address));

        Address = address;
    }

    public static IpAddress Create(string address)
    {
        return new IpAddress(address);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }
}
