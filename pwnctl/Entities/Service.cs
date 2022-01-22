using pwnctl.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace pwnctl.Entities
{
    public class Service : BaseAsset, IAsset
    {
        public TransportProtocol TransportProtocol { get; set; }
        public string ApplicationProtocol { get; set; }
        
        public ushort Port { get; set; }

        public int? HostId { get; set; }
        public Host Host { get; set; }

        public int? DomainId { get; set; }
        public Domain Domain { get; set; }

        public string Origin => HostId.HasValue ? $"{Host.IP}:{Port}" : $"{Domain.Name}:{Port}";

        private Service() {}
        
        public Service(Domain domain, Host host, ushort port, TransportProtocol l4Proto = TransportProtocol.Unknown, string appProto = null)
        {
            Domain = domain;
            Host = host;
            HostId = host == null ? null : 0;
            DomainId = domain == null ? null : 0;
            TransportProtocol = l4Proto;
            ApplicationProtocol = appProto;
            Port = port;
        }

        public static bool TryParse(string assetText, out Service service)
        {
            try
            {
                string strPort = assetText.Split(":").Last();
                var port = ushort.Parse(strPort);
                assetText = assetText.Substring(0, assetText.Length - strPort.Length - 1);

                if (DomainNameParser.IsValid(assetText))
                {
                    service = new Service(new Domain(assetText), null, port);
                    return true;
                }
                else if (IPAddress.TryParse(assetText, out IPAddress address))
                {
                    service = new Service(null, new Host(assetText, address.AddressFamily), port);
                    return true;
                }

                service = null;
                port = 0;
            }
            catch
            {
                service = null;
            }

            return false;
        }
    }

    public enum TransportProtocol
    {
        TCP,
        UDP,
        SCTP,
        Unknown = 99
    }
}
