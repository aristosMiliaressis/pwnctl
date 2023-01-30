using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Common.Extensions;
using pwnctl.app.Notifications.Enums;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.ValueObjects;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Notifications.Entities
{
    public sealed class NotificationRule : Entity<int>
    {
        public string ShortName { get; private init; }
        public AssetClass SubjectClass { get; private set; }
        public NotificationTopic Topic { get; private init; }
        public string Filter { get; private init; }
        public string Template { get; private init; }
        public bool CheckOutOfScope { get; private init; }

        public string Subject { init { SubjectClass = AssetClass.Create(value); } }

        public NotificationRule() { }

        public bool Check(AssetRecord record)
        {
            if (SubjectClass.Class != record.Asset.GetType().Name)
                return false;

            return PwnInfraContext.FilterEvaluator.Evaluate(Filter, record);
        }

        public string GetText(Asset asset)
        {
            if (!string.IsNullOrEmpty(Template))
                return Template.Interpolate(asset);

            return $"{asset} triggered rule {ShortName}";
        }
    }
}