namespace pwnctl.domain.Entities;

using System.Net;
using pwnctl.kernel.Attributes;
using pwnctl.kernel.BaseClasses;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

public sealed class DomainNameRecord : Asset
{
    [EqualityComponent]
    public DnsRecordType Type { get; init; }

    [EqualityComponent]
    public string Key {get; init;}

    [EqualityComponent]
    public string Value { get; init; }

    public Guid? DomainId { get; private init; }
    public Guid? HostId { get; private init; }

    public DomainName? DomainName { get; private init; }
    public NetworkHost? NetworkHost { get; private init; }


    public List<NetworkHost> SPFHosts { get; private set; }

    public DomainNameRecord() {}
    
    public DomainNameRecord(DnsRecordType type, string key, string value)
    {
        Type = type;
        Value = value;

        var result = DomainName.TryParse(key);
        if (!result.IsOk)
            throw new Exception($"unparssable dns record key {key}");

        DomainName = result.Value;
        Key = DomainName.Name;

        var hostResult = NetworkHost.TryParse(value);
        if (hostResult.IsOk)
        {
            NetworkHost = hostResult.Value;
            NetworkHost.AARecords = new List<DomainNameRecord> { this };
        }
    }

    public static Result<DomainNameRecord, string> TryParse(string assetText)
    {
        try
        {
            assetText = assetText.Replace("\t", " ");
            var parts = assetText.Split(" ").Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            if (parts.Length >= 4 && parts[1] == "IN"
                && Enum.GetNames(typeof(DnsRecordType)).Contains(parts[2]))
            {
                var record = new DomainNameRecord(Enum.Parse<DnsRecordType>(parts[2]), parts[0], string.Join(" ", parts.Skip(3)));

                if (record.Type == DnsRecordType.TXT && record.Value.Contains("spf"))
                {
                    record.SPFHosts = DomainNameRecord.ParseSPFString(record.Value);
                }

                return record;
            }

            return $"{assetText} is not a {nameof(DomainNameRecord)}";
        }
        catch
        {
            return $"{assetText} is not a {nameof(DomainNameRecord)}";
        }
    }

    public static List<NetworkHost> ParseSPFString(string spf)
    {
        return spf.Split("ip")
                .Skip(1)
                .Select(p => string.Join(":", p.Split(":").Skip(1)).Trim().Split(" ")[0])
                .Where(ip => IPAddress.TryParse(ip, out IPAddress? _))
                .Select(ip => new NetworkHost(IPAddress.Parse(ip)))
                .ToList();
    }

    public override string ToString()
    {
        return Key + " IN " + Type + " " + Value;
    }
}