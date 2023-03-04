using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Tasks.Entities
{
    public sealed class TaskProfile : Entity<int>
    {
        public string ShortName { get; init; }
        public List<TaskDefinition> TaskDefinitions { get; private set;}

        public TaskProfile() { }

        public TaskProfile(string name, List<TaskDefinition> definitions) 
        {
            ShortName = name;
            TaskDefinitions = definitions;
        }
    }
}