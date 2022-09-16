using System.Text.RegularExpressions;
using pwnctl.core.Entities.Assets;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities
{
    public class ScopeDefinition : BaseEntity<int>
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

        public enum ScopeType
        {
            DomainRegex,
            UrlRegex,
            CIDR
        }
    }
}