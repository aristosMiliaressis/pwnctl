using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using System.Net.Sockets;
using System.Net;

// (25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?(\.|$)){4}
//\b(?:[A-F0-9]{1,4}:){7}[A-F0-9]{1,4}\b

namespace pwnctl.domain.Entities
{
    public sealed class NetworkHost : Asset
    {
        [EqualityComponent]
        public string IP { get; init; }
        public AddressFamily Version { get; init; }

        public List<DomainNameRecord> AARecords { get; internal set; } = new List<DomainNameRecord>();
        public List<NetworkSocket> Sockets { get; private init; }
        
        public NetworkHost() {}

        public NetworkHost(IPAddress address)
        {
            IP = address.ToString();
            Version = address.AddressFamily;
        }

        public static bool TryParse(string assetText, out Asset asset)
        {
            asset = null;

            // if inside square brakets could be an ipv6
            assetText = assetText.StartsWith("[") && assetText.EndsWith("]")
                    ? assetText.Substring(1, assetText.Length-2)
                    : assetText;

            if (assetText.Contains("]"))
                return false;

            if (IPAddress.TryParse(assetText, out IPAddress address))
            {
                var host = new NetworkHost(address);
                asset = host;
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return IP;
        }
    }
}
