using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Common.ValueObjects;
using pwnctl.domain.ValueObjects;
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

        public bool Matches(AssetRecord record)
        {
            if (SubjectClass.Value != record.Asset.GetType().Name)
                return false;

            if (string.IsNullOrEmpty(Filter))
                return true;

            return PwnInfraContext.FilterEvaluator.Evaluate(Filter, record);
        }
    }

    public struct MonitorRules
    {
        public string Schedule { get; set; }
        public string PreCondition { get; set; }
        public string PostCondition { get; set; }
        public string NotificationTemplate { get; set; }
    }
}