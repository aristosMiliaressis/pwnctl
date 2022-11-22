﻿using pwnwrk.domain.Assets.Attributes;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Common.Entities;
using pwnwrk.domain.Common.BaseClasses;
using pwnwrk.domain.Targets.Entities;
using pwnwrk.domain.Assets.DTO;

namespace pwnwrk.domain.Assets.Entities
{
    public sealed class DNSRecord : Asset
    {
        [UniquenessAttribute]
        public RecordType Type { get; init; }

        [UniquenessAttribute]
        public string Key {get; init;}
        public string Value { get; init; }

        public string HostId { get; private init; }
        public string DomainId { get; private init; }

        public Host Host { get; private init; }
        public Domain Domain { get; private init; }

        public DNSRecord() {}
        
        public DNSRecord(RecordType type, string key, string value)
        {
            Type = type;
            Key = key;
            Value = value;

            if (Host.TryParse(key, null, out Asset[] hosts))
            {
                Host = (Host)hosts[0];
            }
            else if (Domain.TryParse(key, null, out Asset[] domains))
            {
                Domain = (Domain)domains[0];
            }

            if (Host.TryParse(value, null, out hosts))
            {
                Host = (Host)hosts[0];
            }
            else if (Domain.TryParse(value, null, out Asset[] domains))
            {
                Domain = (Domain)domains[0];
            }
        }

        public static bool TryParse(string assetText, List<Tag> tags, out Asset[] assets)
        {
            var _assets = new List<Asset>();
            assetText = assetText.Replace("\t", " ");
            var parts = assetText.Split(" ");

            if (parts.Length >= 4 && parts[1] == "IN"
            && Enum.GetNames(typeof(RecordType)).ToList().Contains(parts[2]))
            {
                var record = new DNSRecord(Enum.Parse<RecordType>(parts[2]), parts[0], string.Join(" ", parts.Skip(3)));
                record.AddTags(tags);
                if (record.Type == RecordType.TXT && record.Value.Contains("spf"))
                {
                    var spfHosts = record.Value
                                        .Split(" ")
                                        .Where(p => p.StartsWith("ip"))
                                        .Select(p => new Host(p.Split(":")[1]));
                    _assets.AddRange(spfHosts);
                }

                _assets.Add(record);
                if (record.Host != null) _assets.Add(record.Host);
                if (record.Domain != null) _assets.Add(record.Domain);
                assets = _assets.ToArray();
                return true;
            }

            assets = null;
            return false;
        }

        internal override bool Matches(ScopeDefinition definition)
        {
            return(Host != null && Host.Matches(definition))
                || (Domain != null && Domain.Matches(definition));
        }

        public override AssetDTO ToDTO()
        {
            var dto = new AssetDTO
            {
                Asset = $"{Key} IN {Type} {Value}",
                Tags = new Dictionary<string, object>
                {
                    {"Key", Key},
                    {"Value", Value},
                    {"Type", Type.ToString()}
                },
                Metadata = new Dictionary<string, string>
                {
                    {"InScope", InScope.ToString()},
                    {"FoundAt", FoundAt.ToString()},
                    {"FoundBy", FoundBy }
                }
            };

            Tags.ForEach(t => dto.Tags.Add(t.Name, t.Value));

            return dto;
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