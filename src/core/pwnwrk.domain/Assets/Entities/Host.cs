using pwnwrk.domain.Assets.Attributes;
using pwnwrk.domain.Common.Entities;
using pwnwrk.domain.Targets.Entities;
using pwnwrk.domain.Targets.Enums;
using System.Net.Sockets;
using System.Net;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.DTO;
using System.Text.Json;

namespace pwnwrk.domain.Assets.Entities
{
    public class Host : BaseAsset
    {
        [UniquenessAttribute]
        public string IP { get; private init; }
        public AddressFamily Version { get; private init; }

        public List<DNSRecord> AARecords { get; private init; } = new List<DNSRecord>();

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
            return (definition.Type == ScopeType.CIDR && NetRange.RoutesTo(IP, definition.Pattern))
            || (definition.Type == ScopeType.DomainRegex && AARecords.Any(r => r.Domain.Matches(definition)));
        }

        public override string ToJson()
        {
            var dto = new AssetDTO
            {
                Asset = IP,
                Tags = new Dictionary<string, object>
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

            return JsonSerializer.Serialize(dto);
        }
    }
}
