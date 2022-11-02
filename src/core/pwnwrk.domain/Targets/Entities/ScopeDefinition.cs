using pwnwrk.domain.Targets.Enums;
using pwnwrk.domain.Common.BaseClasses;
using System.Text.Json.Serialization;

namespace pwnwrk.domain.Targets.Entities
{
    public class ScopeDefinition : BaseEntity<int>
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
    }
}