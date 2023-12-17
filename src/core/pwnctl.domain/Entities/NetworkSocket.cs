namespace pwnctl.domain.Entities;

using pwnctl.kernel.Attributes;
using pwnctl.kernel.BaseClasses;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

public sealed class NetworkSocket : Asset
{
    [EqualityComponent]
    public string Address { get; init; }

    public ushort Port { get; init; }
    public TransportProtocol TransportProtocol { get; init; }

    public Guid? NetworkHostId { get; private init; }
    public NetworkHost? NetworkHost { get; init; }

    public Guid? DomainNameId { get; private init; }
    public DomainName? DomainName { get; init; }

    private NetworkSocket() { }

    internal NetworkSocket(DomainName domain, ushort port, TransportProtocol l4Proto = TransportProtocol.TCP)
    {
        DomainName = domain;
        TransportProtocol = l4Proto;
        Port = port;
        Address = l4Proto.ToString().ToLower() + "://" + domain.Name + ":" + port;
    }

    public NetworkSocket(NetworkHost host, ushort port, TransportProtocol l4Proto = TransportProtocol.TCP)
    {
        NetworkHost = host;
        TransportProtocol = l4Proto;
        Port = port;
        Address = l4Proto.ToString().ToLower() + "://" + host.IP + ":" + port;
    }

    public static Result<NetworkSocket, string> TryParse(string assetText)
    {
        try
        {
            var protocol = TransportProtocol.TCP;

            if (assetText.Contains("://"))
            {
                if (!Enum.TryParse<TransportProtocol>(assetText.ToUpper().Split("://")[0], out protocol))
                    return $"{assetText} is not a {nameof(NetworkSocket)}";

                assetText = assetText.Split("://")[1];
            }

            string strPort = assetText.Split(':').Last();
            
            assetText = assetText.Substring(0, assetText.Length - strPort.Length - 1);

            var port = ushort.Parse(strPort);

            var hostResult = NetworkHost.TryParse(assetText);
            var domainResult = DomainName.TryParse(assetText);

            if (hostResult.IsOk)
            {
                return new NetworkSocket(hostResult.Value, port, protocol);
            }
            else if (domainResult.IsOk)
            {
                return new NetworkSocket(domainResult.Value, port, protocol);
            }

            return $"{assetText} is not a {nameof(NetworkSocket)}";
        }
        catch
        {
            return $"{assetText} is not a {nameof(NetworkSocket)}";
        }
    }

    public override string ToString()
    {
        return Address;
    }
}