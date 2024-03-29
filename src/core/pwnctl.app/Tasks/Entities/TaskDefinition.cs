namespace pwnctl.app.Tasks.Entities;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Common.ValueObjects;
using pwnctl.domain.ValueObjects;
using pwnctl.kernel;
using pwnctl.kernel.BaseClasses;

public sealed class TaskDefinition : Entity<int>
{
    public ShortName Name { get; init; }
    public AssetClass Subject { get; init; }
    public string CommandTemplate { get; init; }
    public string? Filter { get; init; }
    public string? StdinQuery { get; init; }
    public bool CheckNotificationRules { get; init; }
    public bool MatchOutOfScope { get; init; }
    public bool ShortLived { get; set; }

    public TaskProfile Profile { get; private init; }
    public int ProfileId { get; private init; }

    public MonitorRules MonitorRules { get; set; }

    public TaskDefinition() { }

    public TaskDefinition(TaskProfile profile)
    {
        Profile = profile;
    }

    public bool Matches(AssetRecord record, bool minitoring = false)
    {
        if (Subject.Value != record.Asset.GetType().Name)
            return false;

        if (minitoring)
        {
            TaskRecord? lastOccurence = record.Tasks.Where(t => t.Definition.Name == Name)
                                                    .OrderBy(t => t.QueuedAt)
                                                    .LastOrDefault();

            if (!MonitorRules.Matches(record, lastOccurence))
                return false;
        }

        if (string.IsNullOrEmpty(Filter))
            return true;

        return PwnInfraContext.FilterEvaluator.Evaluate(Filter, record);
    }
}

public struct MonitorRules
{
    public CronExpression? Schedule { get; init; }
    public string? PreCondition { get; init; }
    public string? PostCondition { get; init; }
    public string? NotificationTemplate { get; init; }

    public bool Matches(AssetRecord record, TaskRecord? lastOccurence)
    {
        if (lastOccurence is not null && Schedule is not null &&
            Schedule.GetNextOccurrence(lastOccurence.QueuedAt) > SystemTime.UtcNow())
        {
            return false;
        }

        if (PreCondition is null)
        {
            return false;
        }

        return PwnInfraContext.FilterEvaluator.Evaluate(PreCondition, record);
    }
}