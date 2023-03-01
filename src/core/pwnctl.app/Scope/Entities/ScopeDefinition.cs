using pwnctl.domain.BaseClasses;
using pwnctl.kernel.BaseClasses;
using System.Text.Json.Serialization;
using pwnctl.domain.Entities;
using System.Text.RegularExpressions;
using pwnctl.app.Scope.Enums;

namespace pwnctl.app.Scope.Entities
{
    public class ScopeDefinition : Entity<int>
    {
        public ScopeType Type { get; init; }
        public string Pattern { get; init; }

        [JsonIgnore]
        public int? ProgramId { get; private init; }
        [JsonIgnore]
        public Program Program { get; private init; }

        public ScopeDefinition() {}

        public ScopeDefinition(Program program)
        {
            Program = program;
        }

        public bool Matches(Asset asset)
        {
            return Type switch
            {
                ScopeType.CIDR => CidrMatchingChecks(asset),
                ScopeType.DomainRegex => DomainMatchingChecks(asset),
                ScopeType.UrlRegex => UrlMatchingChecks(asset),
                _ => false
            };
        }

        private bool CidrMatchingChecks(Asset asset)
        {
            return asset switch
            {
                NetworkRange net => NetworkRange.RoutesTo(net.FirstAddress, Pattern),
                NetworkHost host => NetworkRange.RoutesTo(host.IP, Pattern),
                Email email => Matches(email.DomainName),
                DomainNameRecord record => record.NetworkHost != null && Matches(record.NetworkHost),
                HttpEndpoint ep => Matches(ep.Socket),
                HttpHost vh => Matches(vh.Socket),
                HttpParameter param => Matches(param.Endpoint),
                NetworkSocket srv => srv.NetworkHost != null && Matches(srv.NetworkHost),
                _ => false
            };
        }

        private bool DomainMatchingChecks(Asset asset)
        {
            return asset switch
            {
                DomainNameRecord record => new Regex(Pattern).Matches(record.Key).Count > 0,
                DomainName domain => new Regex(Pattern).Matches(domain.Name).Count > 0,
                Email email => Matches(email.DomainName),
                HttpHost vh => Matches(vh.Socket),
                HttpEndpoint ep => Matches(ep.Socket),
                NetworkHost host => host.AARecords.Any(r => Matches(r.DomainName)),
                HttpParameter param => Matches(param.Endpoint),
                NetworkSocket srv => srv.NetworkHost != null && Matches(srv.NetworkHost) || srv.DomainName != null && Matches(srv.DomainName),
                _ => false
            };
        }

        private bool UrlMatchingChecks(Asset asset)
        {
            return asset switch
            {
                HttpEndpoint ep => new Regex(Pattern).Matches(ep.Url).Count > 0,
                HttpParameter param => Matches(param.Endpoint),
                _ => false
            };
        }
    }
}