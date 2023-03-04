using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

namespace pwnctl.domain.Entities
{
    public sealed class NetworkSocket : Asset
    {
        [EqualityComponent]
        public string Address { get; init; }

        public ushort Port { get; init; }
        public TransportProtocol TransportProtocol { get; init; }

        public string NetworkHostId { get; private init; }
        public NetworkHost NetworkHost { get; init; }

        public string DomainNameId { get; private init; }
        public DomainName DomainName { get; init; }

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

        public static Asset TryParse(string assetText)
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

            if (host != null)
            {
                return new NetworkSocket((NetworkHost) host, port, protocol);
            }
            else if (domain != null)
            {
                return new NetworkSocket((DomainName)domain, port, protocol);
            }

            return null;
        }

        public override string ToString()
        {
            return Address;
        }
    }
}