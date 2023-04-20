namespace pwnctl.dto.Scope.Models;

using pwnctl.app.Scope.Entities;
using pwnctl.app.Scope.Enums;

public class ScopeDefinitionRequestModel
{
    public ScopeType Type { get; set; }
    public string Pattern { get; set; }

    public ScopeDefinitionRequestModel() { }

    public ScopeDefinitionRequestModel(ScopeDefinition definition)
    {
        Type = definition.Type;
        Pattern = definition.Pattern;
    }
}