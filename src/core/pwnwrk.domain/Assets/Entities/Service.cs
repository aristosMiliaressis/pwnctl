using pwnwrk.domain.Assets.Attributes;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.DTO;
using pwnwrk.domain.Common.Entities;
using pwnwrk.domain.Targets.Entities;
using System.Text.Json;

namespace pwnwrk.domain.Assets.Entities
{
    public sealed class Service : Asset
    {
        public ushort Port { get; private init; }

        [UniquenessAttribute]
        public string Origin { get; private init; }

        public TransportProtocol TransportProtocol { get; private init; }
        public string ApplicationProtocol { get; private init; }

        public string HostId { get; private init; }
        public Host Host { get; private init; }

        public string DomainId { get; private init; }
        public Domain Domain { get; private init; }

        private Service() { }

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

        public static bool TryParse(string assetText, List<Tag> tags, out Asset[] assets)
        {
            var _assets = new List<Asset>();
            try
            {
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

                if (Host.TryParse(assetText, null, out Asset[] hostAssets))
                {
                    _assets.Add((Host)hostAssets[0]);
                    var service = new Service((Host)hostAssets[0], port, protocol);
                    service.AddTags(tags);
                    _assets.Add(service);
                    assets = _assets.ToArray();
                    return true;
                }
                else if (Domain.TryParse(assetText, null, out Asset[] domains))
                {
                    _assets.Add((Domain)domains[0]);
                    var service = new Service((Domain)domains[0], port, protocol);
                    service.AddTags(tags);
                    _assets.Add(service);
                    assets = _assets.ToArray();
                    return true;
                }

                assets = null;
                port = 0;
            }
            catch
            {
                assets = null;
            }

            return false;
        }

        internal override bool Matches(ScopeDefinition definition)
        {
            return Host != null && Host.Matches(definition)
                || Domain != null && Domain.Matches(definition);
        }

        public override string ToJson()
        {
            var dto = new AssetDTO
            {
                Asset = Origin,
                Tags = new Dictionary<string, object>
                {
                    {"Port", Port.ToString()},
                    {"Host", Host?.IP},
                    {"Domain", Domain?.Name},
                    {"TransportProtocol", TransportProtocol.ToString()}
                },
                Metadata = new Dictionary<string, string>
                {
                    {"InScope", InScope.ToString()},
                    {"FoundAt", FoundAt.ToString()},
                    {"FoundBy", FoundBy }
                }
            };

            Tags.ForEach(t => dto.Tags.Add(t.Name, t.Value));

            return JsonSerializer.Serialize(dto);
        }
    }
    public enum TransportProtocol
    {
        TCP,
        UDP,
        SCTP,
        Unknown = 99
    }
}