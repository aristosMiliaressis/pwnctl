using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

namespace pwnctl.domain.Entities
{
    public sealed class Service : Asset
    {
        public ushort Port { get; init; }

        [EqualityComponent]
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

        public static bool TryParse(string assetText, out Asset mainAsset, out Asset[] relatedAssets)
        {
            string strPort = assetText.Split(':').Last();
            
            assetText = assetText.Substring(0, assetText.Length - strPort.Length - 1);

            var protocol = strPort[0] switch // TODO: replace this with scheme tcp://, udp:// sctp://
            {
                'U' => TransportProtocol.UDP,
                'T' => TransportProtocol.TCP,
                'S' => TransportProtocol.SCTP,
                _ => TransportProtocol.TCP
            };

            if (!char.IsDigit(strPort[0]))
                strPort = strPort.Substring(1);

            var port = ushort.Parse(strPort);

            if (Host.TryParse(assetText, out Asset host, out Asset[] assets))
            {
                var service = new Service((Host)host, port, protocol);

                mainAsset = service;
                relatedAssets = new Asset[] { host };
                return true;
            }
            else if (Domain.TryParse(assetText, out Asset domain, out assets))
            {
                var service = new Service((Domain)domain, port, protocol);
                mainAsset = service;
                relatedAssets = new Asset[] { domain };
                return true;
            }

            mainAsset = null;
            relatedAssets = null;
            return false;
        }

        public override string ToString()
        {
            return Origin;
        }
    }
}