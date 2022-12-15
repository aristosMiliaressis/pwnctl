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
                DNSRecord record => record.Host != null && Matches(record.Host),
                Endpoint ep => Matches(ep.Service),
                Parameter param => Matches(param.Endpoint),
                Service srv => srv.Host != null && Matches(srv.Host),
                _ => false
            };
        }

        private bool DomainMatchingChecks(Asset asset)
        {
            return asset switch
            {
                DNSRecord record => record.Host != null && Matches(record.Host) || record.Domain != null && Matches(record.Domain),
                Domain domain => new Regex(Pattern).Matches(domain.Name).Count > 0,
                CloudService svc => Matches(svc.Domain),
                Email email => Matches(email.Domain),
                Endpoint ep => Matches(ep.Service),
                Host host => host.AARecords.Any(r => Matches(r.Domain)),
                Keyword word => Matches(word.Domain),
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