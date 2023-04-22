using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Common.ValueObjects;
using pwnctl.domain.ValueObjects;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Tasks.Entities
{
    public sealed class TaskDefinition : Entity<int>
    {
        public ShortName ShortName { get; set; }
        public string CommandTemplate { get; set; }
        public bool IsActive { get; set; }
        public int Aggressiveness { get; set; }
        public AssetClass SubjectClass { get; set; }
        public string Filter { get; set; }
        public bool MatchOutOfScope { get; set; }

        public TaskProfile Profile { get; set; }
        public int ProfileId { get; private init; }

        public string Subject { init { SubjectClass = AssetClass.Create(value); } }
        public string Name { init { ShortName = ShortName.Create(value); } }

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
}