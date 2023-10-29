using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.ValueObjects;

namespace pwnctl.infra.Configuration;

public readonly record struct TaskConfigFile(string Profile, List<TaskDefinitionDTO> TaskDefinitions);

public class TaskDefinitionDTO
{
    public string Name { get; init; }
    public string Subject { get; init; }
    public string CommandTemplate { get; init; }
    public string? StdinQuery { get; init; }
    public string? Filter { get; init; }
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
            Filter = Filter,
            StdinQuery = StdinQuery,
            MatchOutOfScope = MatchOutOfScope,
            CheckNotificationRules = CheckNotificationRules,
        };

        definition.MonitorRules = new MonitorRules
        {
            Schedule = MonitorRules.Schedule == null ? null : CronExpression.Create(MonitorRules.Schedule),
            PreCondition = MonitorRules.PreCondition,
            PostCondition = MonitorRules.PostCondition,
            NotificationTemplate = MonitorRules.NotificationTemplate
        };

        return definition;
    }
}

public readonly record struct MonitorRulesDTO(string? Schedule, string? PreCondition, string? PostCondition, string? NotificationTemplate);
