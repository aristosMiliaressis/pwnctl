using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace pwnctl.Entities
{
    public class NetRange : BaseAsset, IAsset
    {
        public string FirstAddress { get; set; }
        public ushort NetPrefixBits { get; set; }
        public string CIDR => $"{FirstAddress}/{NetPrefixBits}";

        private NetRange() {}
        
        public NetRange(string firstAddress, ushort netPrefix)
        {
            FirstAddress = firstAddress;
            NetPrefixBits = netPrefix;
        }

        public static bool TryParse(string assetText, out NetRange netRange)
        {
            try
            {
                var firstAddress = assetText.Split("/")[0];
                var netPrefixBits = ushort.Parse(assetText.Split("/")[1]);

                netRange = new NetRange(firstAddress, netPrefixBits);

                return true;
            }
            catch
            {
                netRange = null;
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
    }
}
