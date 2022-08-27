using pwnctl.core.Attributes;
using System.Net.Sockets;
using System.Net;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities.Assets
{
    public class Host : BaseAsset
    {
        [UniquenessAttribute]
        public string IP { get; set; }
        public AddressFamily Version { get; set; }

        public List<DNSRecord> AARecords { get; set; } = new List<DNSRecord>();

        private Host() {}

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
                host.Tags = tags;
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
    }
}
