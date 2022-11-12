using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Common.BaseClasses;
using pwnwrk.domain.Assets.Interfaces;

namespace pwnwrk.domain.Notifications.Entities
{
    public sealed class NotificationRule : Entity<int>
    {
        public string ShortName { get; private init; }
        public string Subject { get; private init; }
        public string Filter { get; private init; }
        public string Topic { get; private init; }
        public string Severity { get; private init; }

        public NotificationRule() { }

        public bool Check(Asset asset)
        {
            return Subject == asset.GetType().Name
                && IFilterEvaluator.Instance.Evaluate(Filter, asset);
        }
    }
}