namespace pwnctl.app.Scope.Entities;

using pwnctl.app.Common.ValueObjects;
using pwnctl.kernel.BaseClasses;

public class ScopeAggregate : Entity<int>
{
    public ShortName Name { get; private init; }
    public string? Description { get; private init; }

    public List<ScopeDefinitionAggregate> Definitions { get; set; } = new();

    public ScopeAggregate() {}

    public ScopeAggregate(string name, string description)
    {
        Name = ShortName.Create(name);
        Description = description;
    }
}