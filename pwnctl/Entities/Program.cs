using System.Collections.Generic;

namespace pwnctl.Entities
{
    public class Program
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Platform { get; set; }
        public List<ScopeDefinition> Scope { get; set; } 

        private Program() {}
    }
}