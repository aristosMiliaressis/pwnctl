using pwnctl.kernel.BaseClasses;
using pwnctl.app.Tasks.Entities;

namespace pwnctl.app.Scope.Entities
{
    public sealed class OperationalPolicy : Entity<int>
    {
        public string Blacklist { get; init; }
        public string Whitelist { get; init; }
        public uint? MaxAggressiveness { get; init; }
        public bool AllowActive { get; init; }

        public OperationalPolicy() {}

        public List<TaskDefinition> GetAllowedTasks(List<TaskDefinition> definitions)
        {
            List<TaskDefinition> allowedTasks = new();

            foreach (var definition in definitions)
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

            if (blacklist.Contains(definition.ShortName))
            {
                return false;
            }
            else if (whitelist.Contains(definition.ShortName))
            {
                return true;
            }
            else if (definition.IsActive && !AllowActive)
            {
                return false;
            }
            else if (definition.Aggressiveness <= MaxAggressiveness)
            {
                return true;
            }

            return false;
        }
    }
}