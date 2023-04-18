namespace pwnctl.app.Scope.Entities;

public class ScopeDefinitionAggregate
{
    public int AggregateId { get; set; }
    public ScopeAggregate Aggregate { get; set; }
    public int DefinitionId { get; set; }
    public ScopeDefinition Definition { get; set; }

    private ScopeDefinitionAggregate() {}

    public ScopeDefinitionAggregate(ScopeAggregate aggregate, ScopeDefinition definition) 
    {
        Aggregate = aggregate;
        Definition = definition;
    }
}
