namespace pwnctl.domain.Entities;

using pwnctl.kernel.Attributes;
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

    public NetworkSocket() { }

    public NetworkSocket(DomainName domain, ushort port, TransportProtocol l4Proto = TransportProtocol.TCP)
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

    public static NetworkSocket? TryParse(string assetText)
    {
        var protocol = TransportProtocol.TCP;

        if (assetText.Contains("://"))
        {
            if (!Enum.TryParse<TransportProtocol>(assetText.ToUpper().Split("://")[0], out protocol))
                return null;

            assetText = assetText.Split("://")[1];
        }

        string strPort = assetText.Split(':').Last();
        
        assetText = assetText.Substring(0, assetText.Length - strPort.Length - 1);

        var port = ushort.Parse(strPort);

        var host = NetworkHost.TryParse(assetText);
        var domain = DomainName.TryParse(assetText);

        if (host is not null)
        {
            return new NetworkSocket(host, port, protocol);
        }
        else if (domain is not null)
        {
            return new NetworkSocket(domain, port, protocol);
        }

        return null;
    }

    public override string ToString()
    {
        return Address;
    }
}