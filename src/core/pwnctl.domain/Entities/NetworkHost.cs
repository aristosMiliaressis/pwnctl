namespace pwnctl.domain.Entities;

using pwnctl.kernel.Attributes;
using pwnctl.kernel.BaseClasses;
using pwnctl.domain.BaseClasses;
using System.Net.Sockets;
using System.Net;

public sealed class NetworkHost : Asset
{
    [EqualityComponent]
    public string IP { get; init; }
    public AddressFamily Version { get; init; }

    public List<DomainNameRecord> AARecords { get; internal set; } = new List<DomainNameRecord>();
    
    private NetworkHost() {}

    internal NetworkHost(IPAddress address)
    {
        IP = address.ToString();
        Version = address.AddressFamily;
    }

    public static Result<NetworkHost, string> TryParse(string assetText)
    {
        try
        {
            // if inside square brakets could be an ipv6
            assetText = assetText.StartsWith("[") && assetText.EndsWith("]")
                    ? assetText.Substring(1, assetText.Length-2)
                    : assetText;

            if (assetText.Contains("]"))
                return $"{assetText} is not a {nameof(NetworkHost)}";

            if (!IPAddress.TryParse(assetText, out IPAddress? address))
                return $"{assetText} is not a {nameof(NetworkHost)}";

            return new NetworkHost(address);
        }
        catch
        {
            return $"{assetText} is not a {nameof(NetworkHost)}";
        }
    }

    public override string ToString()
    {
        return IP;
    }
}