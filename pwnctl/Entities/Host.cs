using pwnctl.ValueObject;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace pwnctl.Entities
{
    public class Host : BaseAsset, IAsset
    {
        public string IP { get; set; }
        public AddressFamily Version { get; set; }

        public OperatingSystem OperatingSystem { get; set; }

        public List<DNSRecord> AARecords { get; set; }

        private Host() {}

        public Host(string ip, AddressFamily version)
        {
            IP = ip;
            Version = version;
        }
    }
}
