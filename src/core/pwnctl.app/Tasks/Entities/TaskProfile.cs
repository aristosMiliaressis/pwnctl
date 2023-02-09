using pwnctl.app.Assets.Aggregates;
using pwnctl.domain.ValueObjects;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Tasks.Entities
{
    public sealed class TaskProfile : Entity<int>
    {
        public string ShortName { get; private init; }
        public List<TaskDefinition> TaskDefinitions { get; private set;}

        public TaskProfile() { }

        public TaskProfile(string name, List<TaskDefinition> definitions) 
        {
            ShortName = name;
            TaskDefinitions = definitions;
        }
    }
}