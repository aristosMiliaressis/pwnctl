using System.Text.RegularExpressions;
using pwnwrk.domain.Entities.Assets;
using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities
{
    public class ScopeDefinition : BaseEntity<int>
    {
        public string Name { get; set; }
        public ScopeType Type { get; set; }
        public string Pattern { get; set; }

        public int? ProgramId { get; set; }
        public Program Program { get; set; }

        public ScopeDefinition() {}

        public ScopeDefinition(Program program)
        {
            Program = program;
        }

        public enum ScopeType
        {
            DomainRegex,
            UrlRegex,
            CIDR
        }
    }
}