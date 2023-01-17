using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

namespace pwnctl.domain.Entities
{
    public sealed class Service : Asset
    {
        [EqualityComponent]
        public string Origin { get; init; }

        public ushort Port { get; init; }
        public TransportProtocol TransportProtocol { get; init; }

        public string HostId { get; private init; }
        public Host Host { get; init; }

        public string DomainId { get; private init; }
        public Domain Domain { get; init; }

        public List<Endpoint> Endpoints { get; init; }

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

        public static bool TryParse(string assetText, out Asset asset)
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

            if (Host.TryParse(assetText, out Asset host))
            {
                var service = new Service((Host)host, port, protocol);

                asset = service;
                return true;
            }
            else if (Domain.TryParse(assetText, out Asset domain))
            {
                var service = new Service((Domain)domain, port, protocol);
                asset = service;
                return true;
            }

            asset = null;
            return false;
        }

        public override string ToString()
        {
            return Origin;
        }
    }
}