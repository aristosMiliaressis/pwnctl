using System.Net;
using pwnctl.domain.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

namespace pwnctl.domain.Entities
{
    public sealed class DNSRecord : Asset
    {
        [UniquenessAttribute]
        public DnsRecordType Type { get; init; }

        [UniquenessAttribute]
        public string Key {get; init;}
        public string Value { get; init; }

        public string HostId { get; private init; }
        public string DomainId { get; private init; }

        public Host Host { get; private init; }
        public Domain Domain { get; private init; }

        public DNSRecord() {}
        
        public DNSRecord(DnsRecordType type, string key, string value)
        {
            Type = type;
            Key = key;
            Value = value;

            if (Host.TryParse(key, out Asset host, out Asset[] assets))
            {
                Host = (Host)host;
            }
            else if (Domain.TryParse(key, out Asset domain, out assets))
            {
                Domain = (Domain)domain;
            }

            if (Host.TryParse(value, out host, out assets))
            {
                Host = (Host)host;
            }
            else if (Domain.TryParse(value, out Asset domain, out assets))
            {
                Domain = (Domain)domain;
            }
        }

        public static bool TryParse(string assetText, out Asset mainAsset, out Asset[] relatedAssets)
        {
            var _assets = new List<Asset>();
            assetText = assetText.Replace("\t", " ");
            var parts = assetText.Split(" ");

            if (parts.Length >= 4 && parts[1] == "IN"
            && Enum.GetNames(typeof(DnsRecordType)).ToList().Contains(parts[2]))
            {
                var record = new DNSRecord(Enum.Parse<DnsRecordType>(parts[2]), parts[0], string.Join(" ", parts.Skip(3)));
                mainAsset = record;

                if (record.Type == DnsRecordType.TXT && record.Value.Contains("spf"))
                {
                    var spfHosts = record.Value
                                        .Split("ip")
                                        .Skip(1)
                                        .Select(p => p.Split(":")[1].Trim().Split(" ")[0])
                                        .Where(ip => IPAddress.TryParse(ip, out IPAddress address))
                                        .Select(ip => new Host(IPAddress.Parse(ip)));

                    _assets.AddRange(spfHosts);
                }

                if (record.Host != null) _assets.Add(record.Host);
                if (record.Domain != null) _assets.Add(record.Domain);
                relatedAssets = _assets.ToArray();
                return true;
            }
            
            mainAsset = null;
            relatedAssets = null;
            return false;
        }

        public override string ToString()
        {
            return $"{Key} IN {Type} {Value}";
        }
    }
}
