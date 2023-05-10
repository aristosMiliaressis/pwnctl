using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Common.Extensions;
using pwnctl.app.Tasks.Entities;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Notifications.Entities
{
    public sealed class Notification : Entity<int>
    {
        public AssetRecord Record { get; private init; }
        public Guid RecordId { get; private init; }

        public NotificationRule Rule { get; private init; }
        public int? RuleId { get; private init; }

        public TaskEntry Task { get; set; }
        public int? TaskId { get; private init; }

        public DateTime SentAt { get; set; }

        public Notification() { }

        public Notification(AssetRecord record, NotificationRule rule)
        {
            Record = record;
            Rule = rule;
        }

        public Notification(AssetRecord record, TaskEntry task)
        {
            Record = record;
            Task = task;
            TaskId = task.Id;
        }

        public string GetText()
        {
            if (RuleId.HasValue)
            {
                if (!string.IsNullOrEmpty(Rule.Template))
                    return Rule.Template.Interpolate(Record.Asset);

                return $"{Record.Asset} triggered rule {Rule.ShortName}";
            }

            if (!string.IsNullOrEmpty(Task.Definition.MonitorRules.NotificationTemplate))
            {
                var args = new Dictionary<string, object>
                {
                    { Record.Asset.GetType().Name, Record },
                    { "oldTags", Task.Record },
                    { "newTags", Record }
                };

                //return Task.Definition.MonitorRules.NotificationTemplate.Interpolate(args); // TODO: implement
                return string.Empty;
            }

            return $"new asset {Record.Asset} found by task {Task.Definition.ShortName.Value} on asset {Task.Record.Asset}";
        }
    }
}
