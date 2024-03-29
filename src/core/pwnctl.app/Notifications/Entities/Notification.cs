namespace pwnctl.app.Notifications.Entities;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Common.Extensions;
using pwnctl.app.Tasks.Entities;
using pwnctl.kernel.BaseClasses;

public sealed class Notification : Entity<int>
{
    public AssetRecord Record { get; set; }
    public Guid RecordId { get; set; }

    public NotificationRule? Rule { get; set; }
    public int? RuleId { get; private init; }

    public TaskRecord? Task { get; set; }
    public int? TaskId { get; private init; }

    public DateTime SentAt { get; set; }

    public Notification() { }

    public Notification(AssetRecord record, NotificationRule rule)
    {
        Record = record;
        Rule = rule;
    }

    public Notification(AssetRecord record, TaskRecord task)
    {
        Record = record;
        Task = task;
        TaskId = task.Id;
    }

    public string GetText()
    {
        if (RuleId.HasValue || Rule is not null)
        {
            if (!string.IsNullOrEmpty(Rule?.Template))
                return Rule.Template.Interpolate(Record.Asset, ignoreInvalid: true)
                                    .Replace("Tags", "")
                                    .Interpolate(Record, ignoreInvalid: true);

            return $"{Record.Asset} triggered rule {Rule?.Name.Value}";
        }

        if (!string.IsNullOrEmpty(Task?.Definition?.MonitorRules.NotificationTemplate))
        {
            var message = Task.Definition.MonitorRules.NotificationTemplate.Interpolate(Record.Asset, ignoreInvalid: true);
            message = message.Replace("oldTags", "").Interpolate(Task.Record, ignoreInvalid: true);
            message = message.Replace("newTags", "").Interpolate(Record, ignoreInvalid: true);
            return message;
        }

        return $"new asset {Record.Asset} found by task {Task?.Definition.Name.Value} on asset {Task?.Record.Asset}";
    }
}