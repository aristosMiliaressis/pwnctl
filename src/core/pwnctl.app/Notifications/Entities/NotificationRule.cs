using pwnctl.app.Common.Interfaces;
using pwnctl.domain.BaseClasses;
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

        public bool Check(Asset asset)
        {
            if (SubjectClass.Class != asset.GetType().Name)
                return false;

            return FilterEvaluator.Instance.Evaluate(Filter, asset);
        }
    }
}