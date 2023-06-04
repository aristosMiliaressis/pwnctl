using System.Text.Json.Serialization;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.kernel.BaseClasses;
using pwnctl.app.Common.ValueObjects;

namespace pwnctl.app.Operations.Entities
{
    public class Operation : Entity<int>
    {
        public ShortName ShortName { get; private init; }
        public OperationType Type { get; private init; }
        public OperationState State { get; set; }
        public CronExpression Schedule { get; set; }
        public DateTime InitiatedAt { get; set; }

        [JsonIgnore]
        public int? PolicyId { get; private init; }
        public Policy Policy { get; private init; }

        public ScopeAggregate Scope { get; private init; }
        public int ScopeId { get; private init; }

        public Operation() { }

        public Operation(string name, OperationType type, Policy policy, ScopeAggregate scope)
        {
            ShortName = ShortName.Create(name);
            Type = type;
            Policy = policy;
            Scope = scope;
        }
    }
}
