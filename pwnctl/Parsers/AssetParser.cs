using pwnctl.Entities;
using pwnctl.Exceptions;
using System;
using System.Linq;
using System.Net;

namespace pwnctl.Parsers
{
    public static class AssetParser
    {
        public static BaseAsset Parse(string assetText, out Type assetType)
        {
            if (string.IsNullOrWhiteSpace(assetText))
                throw new ArgumentException("Null or whitespace asset.", nameof(assetText));

            BaseAsset asset;

            assetText = assetText.Split("?")[0];

            // TODO: if scheme is tcp,tcp6,udp,udp6,quic return false & handle as service
            // TODO: DNS record parsing
    
            if (Endpoint.TryParse(assetText, out Endpoint endpoint)) 
            {
                assetType = typeof(Endpoint);
                asset = endpoint;
            }
            else if (Service.TryParse(assetText, out Service service))
            {
                assetType = typeof(Service);
                asset = service;
            }
            else if (IPAddress.TryParse(assetText, out IPAddress address))
            {
                assetType = typeof(Host);
                asset = new Host(assetText, address.AddressFamily);
            }
            else if (DomainNameParser.IsValid(assetText))
            {
                assetType = typeof(Domain);
                asset = new Domain(assetText);
            }
            else if (NetRange.TryParse(assetText, out NetRange netRange))
            {
                assetType = typeof(NetRange);
                asset = netRange;
            }
            else if (DNSRecord.TryParse(assetText, out DNSRecord record))
            {
                assetType = typeof(DNSRecord);
                asset = record;
            }
            else
            {
                throw new UnrecognizableAssetException(assetText);
            }

            return asset;
        }

        public static bool TryParse(string assetText, out Type assetType, out BaseAsset asset)
        {
            try 
            {
                asset = Parse(assetText, out assetType);
                return true;
            }
            catch
            {
                assetType = null;
                asset = null;
                return false;
            }
        }
    }
}