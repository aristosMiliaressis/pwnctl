using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.ValueObjects;

namespace pwnctl.infra.Configuration;

public class TaskConfigFile
{
    public List<string> Profiles { get; set; }
    public List<TaskDefinitionDTO> TaskDefinitions { get; set; }
}

public class TaskDefinitionDTO
{
    public string Name { get; init; }
    public string Subject { get; init; }
    public string CommandTemplate { get; init; }
    public bool IsActive { get; init; }
    public int Aggressiveness { get; init; }
    public string Filter { get; init; }
    public bool MatchOutOfScope { get; init; }
    public bool CheckNotificationRules { get; set; }
    
    public MonitorRulesDTO MonitorRules { get; set; }

    public TaskDefinition ToEntity()
    {
        var definition = new TaskDefinition()
        {
            Name = ShortName.Create(Name),
            Subject = AssetClass.Create(Subject),
            CommandTemplate = CommandTemplate,
            IsActive = IsActive,
            Aggressiveness = Aggressiveness,
            Filter = Filter,
            MatchOutOfScope = MatchOutOfScope,
            CheckNotificationRules = CheckNotificationRules,
        };

        if (MonitorRules != null)
        {
            definition.MonitorRules = new MonitorRules
            {
                Schedule = CronExpression.Create(MonitorRules.Schedule),
                PreCondition = MonitorRules.PreCondition,
                PostCondition = MonitorRules.PostCondition,
                NotificationTemplate = MonitorRules.NotificationTemplate
            };
        }

        return definition;
    }
}

public class MonitorRulesDTO
{
    public string Schedule { get; init; }
    public string PreCondition { get; init; }
    public string PostCondition { get; init; }
    public string NotificationTemplate { get; init; }
}
