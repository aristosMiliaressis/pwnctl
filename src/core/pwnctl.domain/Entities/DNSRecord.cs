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

            if (Host.Parse(key, out Asset[] hosts))
            {
                Host = (Host)hosts[0];
            }
            else if (Domain.Parse(key, out Asset[] domains))
            {
                Domain = (Domain)domains[0];
            }

            if (Host.Parse(value, out hosts))
            {
                Host = (Host)hosts[0];
            }
            else if (Domain.Parse(value, out Asset[] domains))
            {
                Domain = (Domain)domains[0];
            }
        }

        public static bool Parse(string assetText, out Asset[] assets)
        {
            var _assets = new List<Asset>();
            assetText = assetText.Replace("\t", " ");
            var parts = assetText.Split(" ");

            if (parts.Length >= 4 && parts[1] == "IN"
            && Enum.GetNames(typeof(DnsRecordType)).ToList().Contains(parts[2]))
            {
                var record = new DNSRecord(Enum.Parse<DnsRecordType>(parts[2]), parts[0], string.Join(" ", parts.Skip(3)));
                _assets.Add(record);

                if (record.Type == DnsRecordType.TXT && record.Value.Contains("spf"))
                {
                    var spfHosts = record.Value
                                        .Split(" ")
                                        .Where(p => p.StartsWith("ip"))
                                        .Select(p => new Host(p.Split(":")[1]));
                    _assets.AddRange(spfHosts);
                }

                if (record.Host != null) _assets.Add(record.Host);
                if (record.Domain != null) _assets.Add(record.Domain);
                assets = _assets.ToArray();
                return true;
            }

            assets = null;
            return false;
        }

        public override string ToString()
        {
            return $"{Key} IN {Type} {Value}";
        }
    }
}
