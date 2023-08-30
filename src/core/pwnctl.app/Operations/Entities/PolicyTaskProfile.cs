using pwnctl.kernel.BaseClasses;
using pwnctl.app.Tasks.Entities;
using System.Text.Json.Serialization;

namespace pwnctl.app.Operations.Entities
{
    public sealed class PolicyTaskProfile
    {
        public int PolicyId { get; set; }
        public Policy Policy { get; set; }
        public int TaskProfileId { get; set; }
        public TaskProfile TaskProfile { get; set; }       
    }
}
