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

        public List<HttpEndpoint> Endpoints { get; init; }

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

        public static bool TryParse(string assetText, out Asset asset)
        {
            asset = null;
            var protocol = TransportProtocol.TCP;

            if (assetText.Contains("://"))
            {
                if (!Enum.TryParse<TransportProtocol>(assetText.ToUpper().Split("://")[0], out protocol))
                    return false;
                assetText = assetText.Split("://")[1];
            }

            string strPort = assetText.Split(':').Last();
            
            assetText = assetText.Substring(0, assetText.Length - strPort.Length - 1);

            if (!char.IsDigit(strPort[0]))
                strPort = strPort.Substring(1);

            var port = ushort.Parse(strPort);

            if (NetworkHost.TryParse(assetText, out Asset host))
            {
                var service = new NetworkSocket((NetworkHost)host, port, protocol);

                asset = service;
                return true;
            }
            else if (DomainName.TryParse(assetText, out Asset domain))
            {
                var service = new NetworkSocket((DomainName)domain, port, protocol);
                asset = service;
                return true;
            }

            asset = null;
            return false;
        }

        public override string ToString()
        {
            return Address;
        }
    }
}