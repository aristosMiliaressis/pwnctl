using System.Text.Json.Serialization;
using pwnwrk.domain.Common.BaseClasses;

namespace pwnwrk.domain.Targets.Entities
{
    public class Program : Entity<int>
    {
        public string Name { get; init; }
        public string Platform { get; init; }
        [JsonIgnore]
        public int? PolicyId { get; private init; }
        public OperationalPolicy Policy { get; init; }
        public List<ScopeDefinition> Scope { get; init; }

        public Program() {}
    }
}