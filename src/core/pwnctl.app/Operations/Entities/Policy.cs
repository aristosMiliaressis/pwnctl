namespace pwnctl.app.Operations.Entities;

using pwnctl.kernel.BaseClasses;
using pwnctl.app.Tasks.Entities;
using System.Text.Json.Serialization;

public sealed class Policy : Entity<int>
{
    public string? Blacklist { get; set; }

    public List<PolicyTaskProfile> TaskProfiles { get; private init; }

    public Policy() { }

    public Policy(IEnumerable<TaskProfile> profiles)
    {
        if (!profiles.Any())
            throw new Exception("At least one Task Profile is requied for every policy.");
            
        TaskProfiles = profiles.Select(p => new PolicyTaskProfile { Policy = this, TaskProfile = p }).ToList();
    }

    public List<TaskDefinition> GetAllowedTasks()
    {
        List<TaskDefinition> allowedTasks = new();
        var blacklist = Blacklist?.Split(",").Select(t => t.Trim()) ?? new string[0];

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