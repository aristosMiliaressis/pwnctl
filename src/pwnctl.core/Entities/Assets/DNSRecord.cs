using pwnctl.core.Attributes;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities.Assets
{
    public class DNSRecord : BaseAsset
    {
        [UniquenessAttribute]
        public RecordType Type { get; set; }

        [UniquenessAttribute]
        public string Key {get; set;}
        public string Value { get; set; }

        public int? HostId { get; set; }
        public int? DomainId { get; set; }

        public Host Host { get; set; }
        public Domain Domain { get; set; }

        private DNSRecord() {}
        
        public DNSRecord(RecordType type, string key, string value)
        {
            Type = type;
            Key = key;
            Value = value;

            if (Host.TryParse(key, null, out BaseAsset[] hosts))
            {
                Host = (Host)hosts[0];
            }
            else if (Domain.TryParse(key, null, out BaseAsset[] domains))
            {
                Domain = (Domain)domains[0];
            }

            if (Host.TryParse(value, null, out hosts))
            {
                Host = (Host)hosts[0];
            }
            else if (Domain.TryParse(value, null, out BaseAsset[] domains))
            {
                Domain = (Domain)domains[0];
            }
        }

        public static bool TryParse(string assetText, List<Tag> tags, out BaseAsset[] assets)
        {
            var _assets = new List<BaseAsset>();
            assetText = assetText.Replace("\t", " ");
            var parts = assetText.Split(" ");

            if (parts.Length >= 4 && parts[1] == "IN"
            && Enum.GetNames(typeof(RecordType)).ToList().Contains(parts[2]))
            {
                var record = new DNSRecord(Enum.Parse<RecordType>(parts[2]), parts[0], parts[3]);
                record.AddTags(tags);
                _assets.Add(record);
                if (record.Host != null) _assets.Add(record.Host);
                if (record.Domain != null) _assets.Add(record.Domain);
                assets = _assets.ToArray();
                return true;
            }

            assets = null;
            return false;
        }

        public override bool Matches(ScopeDefinition definition)
        {
            return(Host != null && Host.Matches(definition))
                || (Domain != null && Domain.Matches(definition));
        }

        public enum RecordType
        { 
            A,
            AAAA,
            AFSDB,
            APL,
            AXFR,
            CAA,
            CDNSKEY,
            CDS,
            CERT,
            CNAME,
            CSYNC,
            DHCID,
            DNAME,
            DNSKEY,
            DS,
            EUI48,
            EUI64,
            HINFO,
            HIP,
            HTTPS,
            IPSECKEY,
            IXFR,
            KX,
            LOC,
            MX,
            NAPTR,
            NS,
            NSEC,
            NSEC3,
            NSEC3PARAM,
            OPENPGPKEY,
            PTR,
            RP,
            RRSIG,
            SMIMEA,
            SOA,
            SSHFP,
            SVCB,
            SRV,
            TA,
            TKEY,
            TLSA,
            TSIG,
            TXT,
            URI,
            WKS,
            ZONEMD
        }
    }
}
