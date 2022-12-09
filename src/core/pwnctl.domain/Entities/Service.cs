using pwnctl.domain.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

namespace pwnctl.domain.Entities
{
    public sealed class Service : Asset
    {
        public ushort Port { get; init; }

        [UniquenessAttribute]
        public string Origin { get; init; }

        public TransportProtocol TransportProtocol { get; init; }
        public string ApplicationProtocol { get; init; }

        public string HostId { get; private init; }
        public Host Host { get; init; }

        public string DomainId { get; private init; }
        public Domain Domain { get; init; }

        public Service() { }

        public Service(Domain domain, ushort port, TransportProtocol l4Proto = TransportProtocol.TCP)
        {
            Domain = domain;
            TransportProtocol = l4Proto;
            Port = port;
            Origin = l4Proto.ToString().ToLower() + "://" + domain.Name + ":" + port;
        }

        public Service(Host host, ushort port, TransportProtocol l4Proto = TransportProtocol.TCP)
        {
            Host = host;
            HostId = null;
            TransportProtocol = l4Proto;
            Port = port;
            Origin = l4Proto.ToString().ToLower() + "://" + host.IP + ":" + port;
        }

        public static bool Parse(string assetText, out Asset[] assets)
        {
            var _assets = new List<Asset>();

            string strPort = assetText.Split(':').Last();
            
            assetText = assetText.Substring(0, assetText.Length - strPort.Length - 1);

            var protocol = strPort[0] switch
            {
                'U' => TransportProtocol.UDP,
                'T' => TransportProtocol.TCP,
                'S' => TransportProtocol.SCTP,
                _ => TransportProtocol.TCP
            };

            if (!char.IsDigit(strPort[0]))
                strPort = strPort.Substring(1);

            var port = ushort.Parse(strPort);

            if (Host.Parse(assetText, out Asset[] hostAssets))
            {
                var service = new Service((Host)hostAssets[0], port, protocol);
                _assets.Add(service);
                _assets.Add((Host)hostAssets[0]);
                assets = _assets.ToArray();
                return true;
            }
            else if (Domain.Parse(assetText, out Asset[] domains))
            {
                var service = new Service((Domain)domains[0], port, protocol);
                _assets.Add(service);
                _assets.Add((Domain)domains[0]);
                assets = _assets.ToArray();
                return true;
            }

            assets = null;
            port = 0;
            return false;
        }

        public override string ToString()
        {
            return Origin;
        }
    }
}