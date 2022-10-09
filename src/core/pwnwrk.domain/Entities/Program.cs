using System.Collections.Generic;
using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities
{
    public class Program : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Platform { get; set; }
        public int? PolicyId { get; set; }
        public OperationalPolicy Policy { get; set; }
        public List<ScopeDefinition> Scope { get; set; }

        public Program() {}
    }
}