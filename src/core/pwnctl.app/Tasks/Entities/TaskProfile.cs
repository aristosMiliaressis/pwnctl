namespace pwnctl.app.Tasks.Entities;

using pwnctl.app.Common.ValueObjects;
using pwnctl.kernel.BaseClasses;

public sealed class TaskProfile : Entity<int>
{
    public ShortName Name { get; private init; }
    public int Phase { get; private init; }
    public List<TaskDefinition> TaskDefinitions { get; private set;}

    public TaskProfile() { }

    public TaskProfile(string name, int phase, List<TaskDefinition> definitions) 
    {
        Name = ShortName.Create(name);
        Phase = phase;
        TaskDefinitions = definitions;
    }
}