using System.Text.RegularExpressions;

namespace pwnctl.Entities
{
    public class ScopeDefinition : BaseEntity
    {
        public string Name { get; set; }
        public ScopeType Type { get; set; }
        public string Pattern { get; set; }

        public int? ProgramId { get; set; }
        public Program Program { get; set; }

        private ScopeDefinition() {}

        public ScopeDefinition(Program program)
        {
            Program = program;
        }

        public bool Matches(IAsset asset)
        {
            if (typeof(Domain).IsAssignableFrom(asset.GetType()) && Type == ScopeType.DomainRegex)
            {
                var domain = asset as Domain;
                return new Regex(Pattern).Matches(domain.Name).Count > 0;
            }
            else if (typeof(Endpoint).IsAssignableFrom(asset.GetType()) && Type == ScopeType.UrlRegex)
            {
                var endpoint = asset as Endpoint;
                return new Regex(Pattern).Matches(endpoint.Uri).Count > 0;
            }
            else if (typeof(Host).IsAssignableFrom(asset.GetType()) && Type == ScopeType.CIDR)
            {
                var host = asset as Host;
                return NetRange.RoutesTo(host.IP, Pattern);
            }
            else if (typeof(DNSRecord).IsAssignableFrom(asset.GetType()))
            {
                var record = asset as DNSRecord;
                return Matches(record.Host) || Matches(record.Domain);
            }

            return false;
        }

        public enum ScopeType
        {
            DomainRegex,
            UrlRegex,
            CIDR
        }
    }
}