using System;
using System.Collections.Generic;
using System.Linq;

namespace Pwntainer.Application.Entities
{
    public class DNSRecord : BaseAsset
    {
        public RecordType Type { get; set; }

        public string Key {get; set;}
        public string Value { get; set; }

        public int? HostId { get; set; }
        public int? DomainId { get; set; }

        public Host Host { get; set; }
        public Domain Domain { get; set; }

        public static bool IsDNSRecord(string asset)
        {
            asset = asset.Replace("\t", " ");
            var parts = asset.Split(" ");

            return parts.Length >= 4 && parts[1] == "IN" && Enum.GetValues(typeof(RecordType)).OfType<string>().ToList().Contains(parts[2]);
        }
    }

    public enum RecordType
    { 
        A,
        AAAA,
        PTR,
        CNAME,
        NS,
        MX,
        TXT,
        SRV,
        SOA
    }
}
