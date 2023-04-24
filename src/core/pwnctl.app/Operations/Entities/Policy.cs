using pwnctl.kernel.BaseClasses;
using pwnctl.app.Tasks.Entities;
using System.Text.Json.Serialization;

namespace pwnctl.app.Operations.Entities
{
    public sealed class Policy : Entity<int>
    {
        public string Blacklist { get; set; }
        public string Whitelist { get; set; }
        public uint MaxAggressiveness { get; set; }
        public bool OnlyPassive { get; set; }

        [JsonIgnore]
        public int TaskProfileId { get; private init; }
        public TaskProfile TaskProfile { get; private init; }

        public Policy() { }

        public Policy(TaskProfile profile) 
        { 
            TaskProfile = profile;
        }

        public List<TaskDefinition> GetAllowedTasks()
        {
            List<TaskDefinition> allowedTasks = new();

            foreach (var definition in TaskProfile.TaskDefinitions)
            {
                if (Allows(definition))
                {
                    allowedTasks.Add(definition);
                }
            }

            return allowedTasks;
        }

        public bool Allows(TaskDefinition definition)
        {
            var whitelist = Whitelist?.Split(",") ?? new string[0];
            var blacklist = Blacklist?.Split(",") ?? new string[0];

            if (blacklist.Contains(definition.ShortName.Value))
            {
                return false;
            }
            else if (whitelist.Contains(definition.ShortName.Value))
            {
                return true;
            }
            else if (definition.IsActive && OnlyPassive)
            {
                return false;
            }
            else if (definition.Aggressiveness <= MaxAggressiveness || MaxAggressiveness == 0)
            {
                return true;
            }

            return false;
        }
    }
}