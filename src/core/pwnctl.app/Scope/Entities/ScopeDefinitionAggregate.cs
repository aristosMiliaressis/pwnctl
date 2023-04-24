namespace pwnctl.app.Scope.Entities;

public class ScopeDefinitionAggregate
{
    public int AggregateId { get; private init; }
    public ScopeAggregate Aggregate { get; private init; }
    public int DefinitionId { get; private init; }
    public ScopeDefinition Definition { get; private init; }

    private ScopeDefinitionAggregate() {}

    public ScopeDefinitionAggregate(ScopeAggregate aggregate, ScopeDefinition definition) 
    {
        Aggregate = aggregate;
        Definition = definition;
    }
}
