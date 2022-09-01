using pwnctl.core.Attributes;
using System.Net;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities.Assets
{
    public class NetRange : BaseAsset
    {
        [UniquenessAttribute]
        public string FirstAddress { get; set; }
        [UniquenessAttribute]
        public ushort NetPrefixBits { get; set; }

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
    }
}
