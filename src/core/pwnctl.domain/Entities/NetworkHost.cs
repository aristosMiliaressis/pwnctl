using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using System.Net.Sockets;
using System.Net;

namespace pwnctl.domain.Entities
{
    public sealed class NetworkHost : Asset
    {
        [EqualityComponent]
        public string IP { get; init; }
        public AddressFamily Version { get; init; }

        public List<DomainNameRecord> AARecords { get; internal set; } = new List<DomainNameRecord>();
        
        public NetworkHost() {}

        public NetworkHost(IPAddress address)
        {
            IP = address.ToString();
            Version = address.AddressFamily;
        }

        public static NetworkHost? TryParse(string assetText)
        {
            // if inside square brakets could be an ipv6
            assetText = assetText.StartsWith("[") && assetText.EndsWith("]")
                    ? assetText.Substring(1, assetText.Length-2)
                    : assetText;

            if (assetText.Contains("]"))
                return null;

            if (!IPAddress.TryParse(assetText, out IPAddress address))
                return null;

            return new NetworkHost(address);
        }

        public override string ToString()
        {
            return IP;
        }
    }
}
