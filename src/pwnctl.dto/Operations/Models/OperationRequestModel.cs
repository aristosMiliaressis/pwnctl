namespace pwnctl.dto.Operations.Models;

using pwnctl.app.Operations.Enums;
using pwnctl.dto.Scope.Models;

public class OperationRequestModel
{
    public string Name { get; set; }
    public OperationType Type { get; set; }
    public string CronSchedule { get; set; }
    public PolicyModel Policy { get; set; }
    public ScopeRequestModel Scope { get; set; }
    public IEnumerable<string> Input { get; set; }
}

public class PolicyModel
{
    public List<string> TaskProfiles { get; set; }
    public string Blacklist { get; set; }
}
