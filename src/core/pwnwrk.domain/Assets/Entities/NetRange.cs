using pwnwrk.domain.Assets.Attributes;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.DTO;
using pwnwrk.domain.Common.Entities;
using pwnwrk.domain.Targets.Entities;
using pwnwrk.domain.Targets.Enums;
using System.Net;

namespace pwnwrk.domain.Assets.Entities
{
    public sealed class NetRange : Asset
    {
        [UniquenessAttribute]
        public string FirstAddress { get; private init; }
        [UniquenessAttribute]
        public ushort NetPrefixBits { get; private init; }

        public string CIDR => $"{FirstAddress}/{NetPrefixBits}";

        public NetRange() {}
        
        public NetRange(string firstAddress, ushort netPrefix)
        {
            FirstAddress = firstAddress;
            NetPrefixBits = netPrefix;
        }

        public static bool TryParse(string assetText, List<Tag> tags, out Asset[] assets)
        {
            try
            {
                var firstAddress = assetText.Split("/")[0];
                var netPrefixBits = ushort.Parse(assetText.Split("/")[1]);
                var netRange = new NetRange(firstAddress, netPrefixBits);
                netRange.AddTags(tags);

                assets = new Asset[] { netRange };
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

        internal override bool Matches(ScopeDefinition definition)
        {
            return definition.Type == ScopeType.CIDR && NetRange.RoutesTo(FirstAddress, definition.Pattern);
        }

        public override AssetDTO ToDTO()
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

            return dto;
        }
    }
}
