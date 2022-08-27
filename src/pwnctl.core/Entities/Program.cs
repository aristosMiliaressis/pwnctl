using System.Collections.Generic;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities
{
    public class Program : BaseEntity
    {
        public string Name { get; set; }
        public string Platform { get; set; }
        public int? PolicyId { get; set; }
        public OperationalPolicy Policy { get; set; }
        public List<ScopeDefinition> Scope { get; set; } 

        private Program() {}
    }
}