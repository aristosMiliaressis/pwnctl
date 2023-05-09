using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Common.ValueObjects;
using pwnctl.domain.ValueObjects;
using pwnctl.kernel;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Tasks.Entities
{
    public sealed class TaskDefinition : Entity<int>
    {
        public ShortName ShortName { get; private init; }
        public AssetClass SubjectClass { get; private init; }
        public string CommandTemplate { get; init; }
        public bool IsActive { get; init; }
        public int Aggressiveness { get; init; }
        public string Filter { get; init; }
        public bool MatchOutOfScope { get; init; }

        public TaskProfile Profile { get; private init; }
        public int ProfileId { get; private init; }

        public string Subject { init { SubjectClass = AssetClass.Create(value); } }
        public string Name { init { ShortName = ShortName.Create(value); } }

        public MonitorRules MonitorRules { get; set; }

        public TaskDefinition() { }

        public TaskDefinition(TaskProfile profile)
        {
            Profile = profile;
        }

        public bool Matches(AssetRecord record, bool minitoring = false)
        {
            if (SubjectClass.Value != record.Asset.GetType().Name)
                return false;

            if (minitoring)
            {
                TaskEntry lastOccurence = record.Tasks.Where(t => t.Definition.ShortName == ShortName)
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
        public CronExpression CronSchedule { get; private set; }
        public string PreCondition { get; set; }
        public string PostCondition { get; set; }
        public string NotificationTemplate { get; set; }

        public string Schedule
        {
            get
            {
                return CronSchedule?.Value;
            }
            init
            {
                CronSchedule = string.IsNullOrEmpty(value)
                            ? null
                            : CronExpression.Create(value);
            }
        }

        public bool Matches(AssetRecord record, TaskEntry lastOccurence)
        {
            if (lastOccurence != null && CronSchedule != null &&
                CronSchedule.GetNextOccurrence(lastOccurence.QueuedAt) > SystemTime.UtcNow())
            {
                return false;
            }

            return PwnInfraContext.FilterEvaluator.Evaluate(PreCondition, record);
        }
    }
}
