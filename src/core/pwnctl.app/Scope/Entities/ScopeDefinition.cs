namespace pwnctl.app.Scope.Entities;

using pwnctl.domain.BaseClasses;
using pwnctl.kernel.BaseClasses;
using pwnctl.domain.Entities;
using System.Text.RegularExpressions;
using pwnctl.app.Scope.Enums;

public class ScopeDefinition : Entity<int>
{
    public ScopeType Type { get; private init; }
    public string Pattern { get; init; }

    public ScopeDefinition() {}

    public ScopeDefinition(ScopeType type, string pattern)
    {
        Type = type;
        Pattern = pattern;
    }

    public bool Matches(Asset? asset)
    {
        return Type switch
        {
            ScopeType.CIDR => CidrMatchingChecks(asset),
            ScopeType.DomainRegex => DomainMatchingChecks(asset),
            ScopeType.UrlRegex => UrlMatchingChecks(asset),
            _ => false
        };
    }

    private bool CidrMatchingChecks(Asset? asset)
    {
        return asset switch
        {
            NetworkRange net => NetworkRange.RoutesTo(net.FirstAddress, Pattern),
            NetworkHost host => NetworkRange.RoutesTo(host.IP, Pattern),
            NetworkSocket srv => srv.NetworkHost is not null && Matches(srv.NetworkHost),
            DomainNameRecord record => record.NetworkHost is not null && Matches(record.NetworkHost),
            HttpEndpoint ep => Matches(ep.Socket),
            HttpParameter param => Matches(param?.Endpoint),
            _ => false
        };
    }

    private bool DomainMatchingChecks(Asset? asset)
    {
        return asset switch
        {
            DomainName domain => new Regex(Pattern).Matches(domain.Name).Count > 0,
            DomainNameRecord record => new Regex(Pattern).Matches(record.Key).Count > 0,
            Email email => Matches(email.DomainName),
            NetworkHost host => host.AARecords.Any(r => Matches(r.DomainName)),
            NetworkSocket srv => srv.NetworkHost is not null && Matches(srv.NetworkHost) || srv.DomainName is not null && Matches(srv.DomainName),
            HttpEndpoint ep => Matches(ep.Socket),
            HttpParameter param => Matches(param.Endpoint),
            _ => false
        };
    }

    private bool UrlMatchingChecks(Asset? asset)
    {
        return asset switch
        {
            HttpEndpoint ep => new Regex(Pattern).Matches(ep.ToString()).Count > 0,
            HttpParameter param => Matches(param.Endpoint),
            _ => false
        };
    }
}