using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace pwnctl.Entities
{
    public class DNSRecord : BaseAsset, IAsset
    {
        public RecordType Type { get; set; }

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

            var isIp = IPAddress.TryParse(key, out IPAddress address);
            Host = isIp ? new Host(key, address.AddressFamily) : null;
            Domain = isIp ? null : new Domain(key);
        }

        public static bool TryParse(string asset, out DNSRecord record)
        {
            asset = asset.Replace("\t", " ");
            var parts = asset.Split(" ");

            if (parts.Length >= 4 && parts[1] == "IN" && Enum.GetValues(typeof(RecordType)).OfType<string>().ToList().Contains(parts[2]))
            {
                record = new DNSRecord(Enum.Parse<RecordType>(parts[2]), parts[0], parts[3]); 

                return true;
            }

            record = null;
            return false;
        }
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
