using pwnctl.app.Common.ValueObjects;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Scope.Entities
{
    public class ScopeAggregate : Entity<int>
    {
        public ShortName ShortName { get; set; }
        public string Description { get; set; }

        public List<ScopeDefinitionAggregate> Definitions { get; set; } = new();
    }
}