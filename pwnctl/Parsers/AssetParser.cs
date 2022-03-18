using pwnctl.Entities;
using pwnctl.Exceptions;
using System;
using System.Linq;

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
            else if (Host.TryParse(assetText, out Host host))
            {
                assetType = typeof(Host);
                asset = host;
            }
            else if (Domain.TryParse(assetText, out Domain domain))
            {
                assetType = typeof(Domain);
                asset = domain;
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