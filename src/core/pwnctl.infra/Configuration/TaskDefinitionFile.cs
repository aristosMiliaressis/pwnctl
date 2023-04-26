using pwnctl.app.Tasks.Entities;

namespace pwnctl.infra.Configuration;

public class TaskDefinitionFile
{
    public List<string> Profiles { get; set; }
    public List<TaskDefinition> TaskDefinitions { get; set; }
}