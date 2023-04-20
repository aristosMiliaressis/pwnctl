using System.Text.Json.Serialization;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.kernel.BaseClasses;
using pwnctl.app.Common.ValueObjects;

namespace pwnctl.app.Operations.Entities
{
    public class Operation : Entity<int>
    {
        public ShortName ShortName { get; init; }
        public OperationType Type { get; init; }

        [JsonIgnore]
        public int? PolicyId { get; private init; }
        public Policy Policy { get; init; }

        public ScopeAggregate Scope { get; set; }
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