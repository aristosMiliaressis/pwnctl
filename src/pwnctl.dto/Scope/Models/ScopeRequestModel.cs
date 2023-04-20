namespace pwnctl.dto.Scope.Models;

using pwnctl.app.Scope.Entities;

public class ScopeRequestModel
{
    public string ShortName { get; set; }
    public string Description { get; set; }
    public IEnumerable<ScopeDefinitionRequestModel> ScopeDefinitions { get; set; }

    public ScopeRequestModel() { }

    public ScopeRequestModel(ScopeAggregate aggregate)
    {
        ShortName = aggregate.ShortName.Value;
        Description = aggregate.Description;
        ScopeDefinitions = aggregate.Definitions.Select(d => new ScopeDefinitionRequestModel(d.Definition));
    }
}
