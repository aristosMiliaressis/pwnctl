using pwnctl.app.Assets.Entities;
using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Notifications.Enums;
using pwnctl.domain.ValueObjects;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Notifications.Entities
{
    public sealed class NotificationRule : Entity<int>
    {
        public ShortName Name { get; init; }
        public AssetClass Subject { get; init; }
        public NotificationTopic Topic { get; init; }
        public string Filter { get; init; }
        public string? Template { get; init; }
        public bool CheckOutOfScope { get; init; }

        public NotificationRule() { }

        public bool Check(AssetRecord record)
        {
            if (Subject.Value != record.Asset.GetType().Name)
                return false;

            return PwnInfraContext.FilterEvaluator.Evaluate(Filter, record);
        }
    }
}
