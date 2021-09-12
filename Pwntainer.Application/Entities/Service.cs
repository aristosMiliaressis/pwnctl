using System;
using System.Collections.Generic;
using System.Text;

namespace Pwntainer.Application.Entities
{
    public class Service : BaseAsset
    {
        public int Id { get; set; }
        public TransportProtocol TransportProtocol { get; set; }
        public ushort Port { get; set; }
        public string IP { get; set; }
        public string Protocol { get; set; }
    }

    public enum TransportProtocol
    {
        TCP,
        UDP
    }
}
