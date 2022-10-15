using System.Text.RegularExpressions;
using pwnwrk.domain.Entities.Assets;
using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities
{
    public class ScopeDefinition : BaseEntity<int>
    {
        public string Name { get; private init; }
        public ScopeType Type { get; private init; }
        public string Pattern { get; private init; }

        public int? ProgramId { get; private init; }
        public Program Program { get; private init; }

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