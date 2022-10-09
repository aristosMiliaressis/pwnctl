using pwnwrk.domain.Attributes;
using pwnwrk.domain.BaseClasses;
using pwnwrk.domain.Models;
using System.Text.Json;

namespace pwnwrk.domain.Entities.Assets
{
    public class Service : BaseAsset
    {
        public ushort Port { get; set; }

        [UniquenessAttribute]
        public string Origin { get; set; }

        public TransportProtocol TransportProtocol { get; set; }
        public string ApplicationProtocol { get; set; }

        public string HostId { get; set; }
        public Host Host { get; set; }

        public string DomainId { get; set; }
        public Domain Domain { get; set; }

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

        public static bool TryParse(string assetText, List<Tag> tags, out BaseAsset[] assets)
        {
            var _assets = new List<BaseAsset>();
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

                if (Host.TryParse(assetText, null, out BaseAsset[] hostAssets))
                {
                    _assets.Add((Host)hostAssets[0]);
                    var service = new Service((Host)hostAssets[0], port, protocol);
                    service.AddTags(tags);
                    _assets.Add(service);
                    assets = _assets.ToArray();
                    return true;
                }
                else if (Domain.TryParse(assetText, null, out BaseAsset[] domains))
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

        public override bool Matches(ScopeDefinition definition)
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
                    {"Domain", Domain?.ToString()},
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