using pwnctl.app.Common.ValueObjects;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Scope.Entities
{
    public class ScopeAggregate : Entity<int>
    {
        public ShortName ShortName { get; private init; }
        public string? Description { get; private init; }

        public List<ScopeDefinitionAggregate> Definitions { get; set; } = new();

        public ScopeAggregate() {}

        public ScopeAggregate(string name, string description)
        {
            ShortName = ShortName.Create(name);
            Description = description;
        }
    }
}