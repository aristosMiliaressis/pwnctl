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
                _ => throw new NotImplementedException()
            };
        }

        private bool CidrMatchingChecks(Asset asset)
        {
            return asset switch
            {
                NetRange net => NetRange.RoutesTo(net.FirstAddress, Pattern),
                Host host => NetRange.RoutesTo(host.IP, Pattern),
                Email email => Matches(email.Domain),
                DNSRecord record => record.Host != null && Matches(record.Host),
                Endpoint ep => Matches(ep.Service),
                VirtualHost vh => Matches(vh.Service),
                Parameter param => Matches(param.Endpoint),
                Service srv => srv.Host != null && Matches(srv.Host),
                _ => false
            };
        }

        private bool DomainMatchingChecks(Asset asset)
        {
            return asset switch
            {
                DNSRecord record => new Regex(Pattern).Matches(record.Key).Count > 0,
                Domain domain => new Regex(Pattern).Matches(domain.Name).Count > 0,
                Email email => Matches(email.Domain),
                VirtualHost vh => Matches(vh.Service),
                Endpoint ep => Matches(ep.Service),
                Host host => host.AARecords.Any(r => Matches(r.Domain)),
                Parameter param => Matches(param.Endpoint),
                Service srv => srv.Host != null && Matches(srv.Host) || srv.Domain != null && Matches(srv.Domain),
                _ => false
            };
        }

        private bool UrlMatchingChecks(Asset asset)
        {
            return asset switch
            {
                Endpoint ep => new Regex(Pattern).Matches(ep.Url).Count > 0,
                Parameter param => Matches(param.Endpoint),
                _ => false
            };
        }
    }
}