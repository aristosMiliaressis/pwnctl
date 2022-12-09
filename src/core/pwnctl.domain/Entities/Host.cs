using pwnctl.domain.Attributes;
using pwnctl.domain.BaseClasses;
using System.Net.Sockets;
using System.Net;

namespace pwnctl.domain.Entities
{
    public sealed class Host : Asset
    {
        [UniquenessAttribute]
        public string IP { get; init; }
        public AddressFamily Version { get; init; }

        public List<DNSRecord> AARecords { get; private init; } = new List<DNSRecord>();

        public Host() {}

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

        public static bool Parse(string assetText, out Asset[] assets)
        {
            if (IPAddress.TryParse(assetText, out IPAddress address))
            {
                var host = new Host(assetText, address.AddressFamily);
                assets = new Asset[] { host };
                return true;
            }

            assets = null;
            return false;
        }

        public override string ToString()
        {
            return IP;
        }
    }
}
