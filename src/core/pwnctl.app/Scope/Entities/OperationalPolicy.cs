using pwnctl.domain.BaseClasses;
using pwnctl.app.Tasks.Entities;

namespace pwnctl.app.Scope.Entities
{
    public sealed class OperationalPolicy : Entity<int>
    {
		public string Blacklist { get; init; }
		public string Whitelist { get; init; }
		public int? MaxAggressiveness { get; init; }
		public bool AllowActive { get; init; }

        public OperationalPolicy() {}

        public List<TaskDefinition> GetAllowedTasks(List<TaskDefinition> definitions)
        {
            List<TaskDefinition> allowedTasks = new();

            var whitelist = Whitelist?.Split(",") ?? new string[0];
            var blacklist = Blacklist?.Split(",") ?? new string[0];

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
                else if (definition.IsActive && !AllowActive)
                {
                    continue;
                }
                else if (definition.Aggressiveness <= MaxAggressiveness)
                {
                    allowedTasks.Add(definition);
                }
            }

            return allowedTasks;
        }
    }
}