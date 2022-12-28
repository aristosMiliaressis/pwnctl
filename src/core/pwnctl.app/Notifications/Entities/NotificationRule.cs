using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Common.Interfaces;
using pwnctl.domain.ValueObjects;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Notifications.Entities
{
    public sealed class NotificationRule : Entity<int>
    {
        public string ShortName { get; private init; }
        public AssetClass SubjectClass { get; private set; }
        public string Filter { get; private init; }
        public string Topic { get; private init; }

        public string Subject { init { SubjectClass = AssetClass.Create(value); } }

        public NotificationRule() { }

        public bool Check(AssetRecord record)
        {
            if (SubjectClass.Class != record.Asset.GetType().Name)
                return false;

            return FilterEvaluator.Instance.Evaluate(Filter, record);
        }
    }
}