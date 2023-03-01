using System.Text.Json.Serialization;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Tasks.Entities;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Scope.Entities
{
    public class Program : Entity<int>
    {
        public string Name { get; init; }
        public string Platform { get; init; }

        [JsonIgnore]
        public int? PolicyId { get; private init; }
        public OperationalPolicy Policy { get; init; }
        [JsonIgnore]
        public int TaskProfileId { get; init; }
        public TaskProfile TaskProfile { get; set; }
        public List<ScopeDefinition> Scope { get; init; }
        public List<AssetRecord> Assets { get; init; }

        public Program() { }

        public Program(string name, TaskProfile profile, OperationalPolicy policy, List<ScopeDefinition> scope) 
        { 
            Name = name;
            TaskProfile = profile;
            Policy = policy;
            Scope = scope;
        }

        public List<TaskDefinition> GetAllowedTasks()
        {
            return Policy.GetAllowedTasks(TaskProfile.TaskDefinitions);
        }
    }
}