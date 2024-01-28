namespace pwnctl.domain.Entities;

using pwnctl.domain.BaseClasses;
using pwnctl.kernel.Attributes;
using pwnctl.kernel.BaseClasses;

public sealed class VirtualHost : Asset
{
    [EqualityComponent]
    public string SocketAddress { get; set; }
    [EqualityComponent]
    public string Hostname { get; set; }

    public NetworkSocket Socket { get; set; }
    public Guid SocketId { get; set; }
    public DomainName Domain { get; set; }
    public Guid DomainId { get; set; }

    private VirtualHost() { }

    private VirtualHost(NetworkSocket sockAddr, DomainName hostname) 
    { 
        Socket = sockAddr;
        SocketAddress = sockAddr.Address;
        Domain = hostname;
        Hostname = hostname.Name;
    }

    public static Result<VirtualHost, string> TryParse(string assetText)
    {
        try
        {
            var parts = assetText.Split('\t');
            if (parts.Length != 2)
                return $"{assetText} is not a {nameof(VirtualHost)}";
            
            var socketResult = NetworkSocket.TryParse(parts[0]);
            if (socketResult.Failed)
                return $"{assetText} is not a {nameof(VirtualHost)}";
                
            var hostnameResult = DomainName.TryParse(parts[1]);
            if (hostnameResult.Failed)
                return $"{assetText} is not a {nameof(VirtualHost)}";

            return new VirtualHost(socketResult.Value, hostnameResult.Value);
        }
        catch
        {
            return $"{assetText} is not a {nameof(VirtualHost)}";
        }
    }

    public override string ToString()
    {
        return $"{SocketAddress}\t{Hostname}";
    }
}