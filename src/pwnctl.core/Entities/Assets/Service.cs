﻿using pwnctl.core.Attributes;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities.Assets
{
    public class Service : BaseAsset
    {
        public ushort Port { get; set; }

        [UniquenessAttribute]
        public string Origin { get; set; }
 
        public TransportProtocol TransportProtocol { get; set; }
        public string ApplicationProtocol { get; set; }
       
        public int? HostId { get; set; }
        public Host Host { get; set; }

        public int? DomainId { get; set; }
        public Domain Domain { get; set; }

        private Service() {}

        public Service(Domain domain, ushort port, TransportProtocol l4Proto = TransportProtocol.Unknown)
        {
            Domain = domain;
            TransportProtocol = l4Proto;
            Port = port;
            Origin = domain.Name + ":" + port;
        }

        public Service(Host host, ushort port, TransportProtocol l4Proto = TransportProtocol.Unknown)
        {
            Host = host;
            HostId = 0;
            TransportProtocol = l4Proto;
            Port = port;
            Origin = host.IP + ":" + port;
        }

        public static bool TryParse(string assetText, List<Tag> tags, out BaseAsset[] assets)
        {
            var _assets = new List<BaseAsset>();
            try
            {
                string strPort = assetText.Split(':').Last();
                assetText = assetText.Substring(0, assetText.Length - strPort.Length - 1);

                var protocol = strPort[0] switch
                {
                    'U' => TransportProtocol.UDP,
                    'T' => TransportProtocol.TCP,
                    'S' => TransportProtocol.SCTP,
                    _ => TransportProtocol.TCP
                };

                if (!char.IsDigit(strPort[0]))
                    strPort = strPort.Substring(1);

                var port = ushort.Parse(strPort);

                if (Host.TryParse(assetText, null, out BaseAsset[] hostAssets))
                {
                    _assets.Add((Host)hostAssets[0]);
                    var service = new Service((Host)hostAssets[0], port, protocol);
                    service.AddTags(tags);
                    _assets.Add(service);
                    assets = _assets.ToArray();
                    return true;
                }
                else if (Domain.TryParse(assetText, null, out BaseAsset[] domains))
                {
                    _assets.Add((Domain)domains[0]);
                    var service = new Service((Domain)domains[0], port, protocol);
                    service.AddTags(tags);
                    _assets.Add(service);
                    assets = _assets.ToArray();
                    return true;
                }

                assets = null;
                port = 0;
            }
            catch
            {
                assets = null;
            }

            return false;
        }

        public override bool Matches(ScopeDefinition definition)
        {
            return Host != null && Host.Matches(definition) 
                || Domain != null && Domain.Matches(definition);
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
