using pwnctl.domain.Attributes;
using pwnctl.domain.BaseClasses;
using System.Net;

namespace pwnctl.domain.Entities
{
    public sealed class NetRange : Asset
    {
        [UniquenessAttribute]
        public string FirstAddress { get; init; }
        [UniquenessAttribute]
        public ushort NetPrefixBits { get; init; }

        public NetRange() {}
        
        public NetRange(IPAddress firstAddress, ushort netPrefix)
        {
            FirstAddress = firstAddress.ToString();
            NetPrefixBits = netPrefix;
        }

        public static bool Parse(string assetText, out Asset[] assets)
        {
            var firstAddress = IPAddress.Parse(assetText.Split("/")[0]);
            var netPrefixBits = ushort.Parse(assetText.Split("/")[1]);
            var netRange = new NetRange(firstAddress, netPrefixBits);

            assets = new Asset[] { netRange };
            return true;
        }

        public static bool RoutesTo(string ipAddress, string cidr)
        {
            string[] parts = cidr.Split('/');

            int ipAddr = BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), 0);
            int cidrAddr = BitConverter.ToInt32(IPAddress.Parse(parts[0]).GetAddressBytes(), 0);
            int cidrMask = IPAddress.HostToNetworkOrder(-1 << (32 - int.Parse(parts[1])));

            return ((ipAddr & cidrMask) == (cidrAddr & cidrMask));
        }

        public override string ToString()
        {
            return $"{FirstAddress}/{NetPrefixBits}";
        }
    }
}
