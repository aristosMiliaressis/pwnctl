using pwnctl.app.Scope.Entities;

namespace pwnctl.infra.Configuration;

public class TargetFile
{
    public string Name { get; set; }
    public string TaskProfile { get; set; }
    public OperationalPolicy Policy { get; set; }
    public List<ScopeDefinition> Scope { get; set; }
}