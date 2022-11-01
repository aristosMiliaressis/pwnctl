using System.Text.Json.Serialization;
using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities
{
    public class Program : BaseEntity<int>
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