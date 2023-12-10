namespace pwnctl.app.Operations.Entities;

using System.Text.Json.Serialization;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.kernel.BaseClasses;
using pwnctl.kernel;
using pwnctl.app.Common.ValueObjects;

public class Operation : Entity<int>
{
    public ShortName Name { get; private init; }
    public OperationType Type { get; private init; }
    public OperationState State { get; private set; }
    public CronExpression? Schedule { get; set; }
    public DateTime InitiatedAt { get; private set; }
    public DateTime FinishedAt { get; private set; }
    public int CurrentPhase { get; private set; }

    [JsonIgnore]
    public int PolicyId { get; private init; }
    public Policy Policy { get; private init; }

    public ScopeAggregate Scope { get; private init; }
    public int ScopeId { get; private init; }

    public Operation() { }

    public Operation(string name, OperationType type, Policy policy, ScopeAggregate scope)
    {
        Name = ShortName.Create(name);
        Type = type;
        Policy = policy;
        Scope = scope;
        State = OperationState.Pending;
        CurrentPhase = policy.TaskProfiles.OrderBy(p => p.TaskProfile.Phase).First().TaskProfile.Phase;
    }

    public void Initialize() 
    {
        State = OperationState.Ongoing;
        InitiatedAt = SystemTime.UtcNow();
    }

    public void Terminate() 
    {
        State = OperationState.Completed;
        FinishedAt = SystemTime.UtcNow();
    }

    public void Cancel() 
    {
        State = OperationState.Cancelled;
        FinishedAt = SystemTime.UtcNow();
    }

    public void TransitionPhase() 
    {
        var nextPhase = Policy.TaskProfiles
                            .Where(p => p.TaskProfile.Phase > CurrentPhase)
                            .OrderBy(p => p.TaskProfile.Phase)
                            .FirstOrDefault().TaskProfile.Phase;

        CurrentPhase = nextPhase;
    }
}