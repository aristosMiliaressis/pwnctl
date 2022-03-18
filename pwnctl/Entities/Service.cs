using pwnctl.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
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

                if (Domain.TryParse(assetText, out Domain domain))
                {
                    service = new Service(domain, null, port);
                    return true;
                }
                else if (Host.TryParse(assetText, out Host host))
                {
                    service = new Service(null, host, port);
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
