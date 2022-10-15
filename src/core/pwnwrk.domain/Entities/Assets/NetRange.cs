using pwnwrk.domain.Attributes;
using pwnwrk.domain.BaseClasses;
using pwnwrk.domain.Models;
using System.Net;
using System.Text.Json;

namespace pwnwrk.domain.Entities.Assets
{
    public class NetRange : BaseAsset
    {
        [UniquenessAttribute]
        public string FirstAddress { get; private init; }
        [UniquenessAttribute]
        public ushort NetPrefixBits { get; private init; }

        public string CIDR => $"{FirstAddress}/{NetPrefixBits}";
    
        private NetRange() {}
        
        public NetRange(string firstAddress, ushort netPrefix)
        {
            FirstAddress = firstAddress;
            NetPrefixBits = netPrefix;
        }

        public static bool TryParse(string assetText, List<Tag> tags, out BaseAsset[] assets)
        {
            try
            {
                var firstAddress = assetText.Split("/")[0];
                var netPrefixBits = ushort.Parse(assetText.Split("/")[1]);
                var netRange = new NetRange(firstAddress, netPrefixBits);
                netRange.AddTags(tags);

                assets = new BaseAsset[] { netRange };
                return true;
            }
            catch
            {
                assets = null;
                return false;
            }
        }

        public static bool RoutesTo(string ipAddress, string cidr)
        {
            string[] parts = cidr.Split('/');

            int ipAddr = BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), 0);
            int cidrAddr = BitConverter.ToInt32(IPAddress.Parse(parts[0]).GetAddressBytes(), 0);
            int cidrMask = IPAddress.HostToNetworkOrder(-1 << (32 - int.Parse(parts[1])));

            return ((ipAddr & cidrMask) == (cidrAddr & cidrMask));
        }

        public override bool Matches(ScopeDefinition definition)
        {
            return definition.Type == ScopeDefinition.ScopeType.CIDR && NetRange.RoutesTo(FirstAddress, definition.Pattern);
        }

        public override string ToJson()
        {
            var dto = new AssetDTO
            {
                Asset = CIDR,
                Tags = new Dictionary<string, object>
                {
                    {"FirstAddress", FirstAddress},
                    {"NetPrefixBits", NetPrefixBits.ToString()}
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
