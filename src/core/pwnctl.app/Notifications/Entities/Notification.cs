using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Common.Extensions;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Notifications.Entities
{
    public sealed class Notification : Entity<int>
    {
        public AssetRecord Record { get; private init; }
        public Guid RecordId { get; private init; }

        public NotificationRule Rule { get; private init; }
        public int RuleId { get; private init; }

        public DateTime SentAt { get; set; }

        public Notification() { }

        public Notification(AssetRecord record, NotificationRule rule) 
        { 
            Record = record;
            Rule = rule;
        }

        public string GetText()
        {
            if (!string.IsNullOrEmpty(Rule.Template))
                return Rule.Template.Interpolate(Record.Asset);

            return $"{Record.Asset} triggered rule {Rule.ShortName}";
        }
    }
}