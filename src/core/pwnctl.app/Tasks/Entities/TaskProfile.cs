using pwnctl.app.Common.ValueObjects;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Tasks.Entities
{
    public sealed class TaskProfile : Entity<int>
    {
        public ShortName ShortName { get; private init; }
        public List<TaskDefinition> TaskDefinitions { get; private set;}

        public TaskProfile() { }

        public TaskProfile(string name, List<TaskDefinition> definitions) 
        {
            ShortName = ShortName.Create(name);
            TaskDefinitions = definitions;
        }
    }
}