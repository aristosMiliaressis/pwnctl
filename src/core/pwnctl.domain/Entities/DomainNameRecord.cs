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

    private DomainNameRecord() {}

    internal DomainNameRecord(DnsRecordType type, string key, string value)
    {
        Type = type;
        Value = value;

        var result = DomainName.TryParse(key);
        if (result.Failed)
            throw new Exception(result.Error);

        DomainName = result.Value;
        Key = DomainName.Name;

        var hostResult = NetworkHost.TryParse(value);
        if(Type == DnsRecordType.PTR)
        {
            var ip = string.Join(".", Key.Split(".").Reverse().Skip(2));
            hostResult = NetworkHost.TryParse(ip);
        }

        if (!hostResult.Failed)
        {
            NetworkHost = hostResult.Value;
            NetworkHost.AARecords = new List<DomainNameRecord> { this };
        }
    }

    public static Result<DomainNameRecord, string> TryParse(string assetText)
    {
        try
        {
            assetText = assetText.Trim().Replace("\t", " ");
            
            var parts = assetText.Split(" ");
            if (parts.Length < 4)
                return $"{assetText} is not a {nameof(DomainNameRecord)}, too few parts";

            var key = parts[0];
            assetText = assetText.Substring(key.Length).Trim();
            parts = assetText.Split(" ");
            if (parts[0] != "IN")
                return $"{assetText} is not a {nameof(DomainNameRecord)}, missing 'IN' keyword";

            assetText = assetText.Substring(parts[0].Length).Trim();
            parts = assetText.Split(" ");
            if (!Enum.TryParse(parts[0], out DnsRecordType type))
            {
                return $"{assetText} is not a {nameof(DomainNameRecord)}, invalid record type '{parts[0]}'";
            }

            var value = assetText.Substring(parts[0].Length).Trim();

            var record = new DomainNameRecord(type, key, value);

            if (record.Type == DnsRecordType.TXT && record.Value.Contains("spf"))
            {
                record.SPFHosts = ParseSPFString(record.Value);
            }

            return record;
        }
        catch (Exception ex)
        {
            return $"{assetText} is not a {nameof(DomainNameRecord)}, {ex.Message}";
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