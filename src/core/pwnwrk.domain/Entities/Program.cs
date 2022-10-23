using System.Text.Json.Serialization;
using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities
{
    public class Program : BaseEntity<int>
    {
        public string Name { get; private init; }
        public string Platform { get; private init; }
        [JsonIgnore]
        public int? PolicyId { get; private init; }
        public OperationalPolicy Policy { get; private init; }
        public List<ScopeDefinition> Scope { get; private init; }

        public Program() {}
    }
}