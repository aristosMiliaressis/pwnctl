using System.Text.Json.Serialization;
using pwnwrk.domain.Common.BaseClasses;
using pwnwrk.domain.Tasks.Entities;

namespace pwnwrk.domain.Targets.Entities
{
    public class Program : Entity<int>
    {
        public string Name { get; init; }
        public string Platform { get; init; }
        [JsonIgnore]
        public int? PolicyId { get; private init; }
        public OperationalPolicy Policy { get; init; }
        public List<ScopeDefinition> Scope { get; init; }

        public Program() {}

        public List<TaskDefinition> GetAllowedTasks(List<TaskDefinition> definitions)
        {
            List<TaskDefinition> allowedTasks = new();

            var whitelist = Policy.Whitelist?.Split(",") ?? new string[0];
            var blacklist = Policy.Blacklist?.Split(",") ?? new string[0];

            foreach (var definition in definitions)
            {
                if (blacklist.Contains(definition.ShortName))
                {
                    continue;
                }
                else if (whitelist.Contains(definition.ShortName))
                {
                    allowedTasks.Add(definition);
                    continue;
                }
                else if (definition.IsActive && !Policy.AllowActive)
                {
                    continue;
                }
                else if (definition.Aggressiveness <= Policy.MaxAggressiveness)
                {
                    allowedTasks.Add(definition);
                }
            }

            return allowedTasks;
        }
    }
}