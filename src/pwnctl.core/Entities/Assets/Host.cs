using pwnctl.core.Attributes;
using System.Net.Sockets;
using System.Net;
using pwnctl.core.BaseClasses;
using pwnctl.core.Models;
using Newtonsoft.Json;

namespace pwnctl.core.Entities.Assets
{
    public class Host : BaseAsset
    {
        [UniquenessAttribute]
        public string IP { get; set; }
        public AddressFamily Version { get; set; }

        public List<DNSRecord> AARecords { get; set; } = new List<DNSRecord>();

        private Host() {}

        public Host(string ip)
        {
            if (!IPAddress.TryParse(ip, out IPAddress address))
                throw new ArgumentException($"{ip} not a valid ip", nameof(ip));

            IP = ip;
            Version = address.AddressFamily;
        }

        public Host(string ip, AddressFamily version = AddressFamily.InterNetwork)
        {
            IP = ip;
            Version = version;
        }

        public static bool TryParse(string assetText, List<Tag> tags, out BaseAsset[] assets)
        {
            if (IPAddress.TryParse(assetText, out IPAddress address))
            {
                var host = new Host(assetText, address.AddressFamily);
                host.AddTags(tags);
                assets = new BaseAsset[] { host };
                return true;
            }

            assets = null;
            return false;
        }

        public override bool Matches(ScopeDefinition definition)
        {
            return (definition.Type == ScopeDefinition.ScopeType.CIDR && NetRange.RoutesTo(IP, definition.Pattern))
            || (definition.Type == ScopeDefinition.ScopeType.DomainRegex && AARecords.Any(r => r.Domain.Matches(definition)));
        }

        public override string ToJson()
        {
            var dto = new AssetDTO
            {
                Asset = IP,
                Tags = new Dictionary<string, string>
                {
                    {"Version", Version.ToString()}
                },
                Metadata = new Dictionary<string, string>
                {
                    {"InScope", InScope.ToString()},
                    {"FoundAt", FoundAt.ToString()},
                    {"FoundBy", FoundBy }
                }
            };

            Tags.ForEach(t => dto.Tags.Add(t.Name, t.Value));

            return JsonConvert.SerializeObject(dto);
        }
    }
}
