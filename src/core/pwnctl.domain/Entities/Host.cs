using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using System.Net.Sockets;
using System.Net;

namespace pwnctl.domain.Entities
{
    public sealed class Host : Asset
    {
        [EqualityComponent]
        public string IP { get; init; }
        public AddressFamily Version { get; init; }

        public List<DNSRecord> AARecords { get; private init; } = new List<DNSRecord>();

        public Host() {}

        public Host(IPAddress address)
        {
            IP = address.ToString();
            Version = address.AddressFamily;
        }

        public static bool TryParse(string assetText, out Asset mainAsset, out Asset[] relatedAssets)
        {
            mainAsset = null;
            relatedAssets = null;

            if (IPAddress.TryParse(assetText, out IPAddress address))
            {
                var host = new Host(address);
                mainAsset = host;
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
