using pwnctl.kernel.BaseClasses;
using pwnctl.app.Tasks.Entities;
using System.Text.Json.Serialization;

namespace pwnctl.app.Operations.Entities
{
    public sealed class Policy : Entity<int>
    {
        public string? Blacklist { get; set; }

        public List<PolicyTaskProfile> TaskProfiles { get; private init; }

        public Policy() { }

        public Policy(List<TaskProfile> profiles)
        {
            TaskProfiles = profiles.Select(p => new PolicyTaskProfile { Policy = this, TaskProfile = p }).ToList();
        }

        public List<TaskDefinition> GetAllowedTasks()
        {
            List<TaskDefinition> allowedTasks = new();
            var blacklist = Blacklist?.Split(",") ?? new string[0];

            foreach (var definition in TaskProfiles.SelectMany(p => p.TaskProfile.TaskDefinitions))
            {
                if (!blacklist.Contains(definition.Name.Value))
                {
                    allowedTasks.Add(definition);
                }
            }

            return allowedTasks;
        }
    }
}
