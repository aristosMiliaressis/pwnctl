using System.Net;
using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

namespace pwnctl.domain.Entities
{
    public sealed class DNSRecord : Asset
    {
        [EqualityComponent]
        public DnsRecordType Type { get; init; }

        [EqualityComponent]
        public string Key {get; init;}
        public string Value { get; init; }

        public string HostId { get; private init; }
        public string DomainId { get; private init; }

        public Host Host { get; private init; }
        public Domain Domain { get; private init; }

        public List<Host> SPFHosts { get; private set; }

        public DNSRecord() {}
        
        public DNSRecord(DnsRecordType type, string key, string value)
        {
            Type = type;
            Key = key;
            Value = value;

            if (Host.TryParse(key, out Asset host))
            {
                Host = (Host)host;
            }
            else if (Domain.TryParse(key, out Asset domain))
            {
                Domain = (Domain)domain;
            }

            if (Host.TryParse(value, out host))
            {
                Host = (Host)host;
                Host.AARecords = new List<DNSRecord> { this };
            }
            else if (Domain.TryParse(value, out Asset domain))
            {
                Domain = (Domain)domain;
            }
        }

        public static bool TryParse(string assetText, out Asset asset)
        {
            assetText = assetText.Replace("\t", " ");
            var parts = assetText.Split(" ");

            if (parts.Length >= 4 && parts[1] == "IN"
             && Enum.GetNames(typeof(DnsRecordType)).Contains(parts[2]))
            {
                var record = new DNSRecord(Enum.Parse<DnsRecordType>(parts[2]), parts[0], string.Join(" ", parts.Skip(3)));

                if (record.Type == DnsRecordType.TXT && record.Value.Contains("spf"))
                {
                    record.SPFHosts = record.Value
                                        .Split("ip")
                                        .Skip(1)
                                        .Select(p => string.Join(":", p.Split(":").Skip(1)).Trim().Split(" ")[0])
                                        .Where(ip => IPAddress.TryParse(ip, out IPAddress address))
                                        .Select(ip => new Host(IPAddress.Parse(ip)))
                                        .ToList();
                }

                asset = record;
                return true;
            }

            asset = null;
            return false;
        }

        public override string ToString()
        {
            return $"{Key} IN {Type} {Value}";
        }
    }
}
