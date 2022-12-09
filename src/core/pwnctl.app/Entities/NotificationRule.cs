using pwnctl.app.Interfaces;
using pwnctl.domain.BaseClasses;

namespace pwnctl.app.Entities
{
    public sealed class NotificationRule : Entity<int>
    {
        public string ShortName { get; private init; }
        public string Subject { get; private init; }
        public string Filter { get; private init; }
        public string Topic { get; private init; }

        public NotificationRule() { }

        public bool Check(Asset asset)
        {
            if (Subject != asset.GetType().Name)
                return false;

            return FilterEvaluator.Instance.Evaluate(Filter, asset);
        }
    }
}