using pwnctl.ValueObject;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace pwnctl.Entities
{
    public class Host : BaseAsset, IAsset
    {
        public string IP { get; set; }
        public AddressFamily Version { get; set; }

        public List<DNSRecord> AARecords { get; set; }

        private Host() {}

        public Host(string ip, AddressFamily version)
        {
            IP = ip;
            Version = version;
        }

        public static bool TryParse(string assetText, out Host host)
        {
            if (IPAddress.TryParse(assetText, out IPAddress address))
            {
                host = new Host(assetText, address.AddressFamily);
                return true;
            }

            host = null;
            return false;
        }
    }
}
