using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Notifications.Enums;
using pwnctl.domain.ValueObjects;

namespace pwnctl.infra.Configuration;

public readonly record struct NotificationRuleDTO
{
    public string Name { get; private init; }
    public string Subject { get; private init; }
    public NotificationTopic Topic { get; private init; }
    public string Filter { get; private init; }
    public string? Template { get; private init; }
    public bool CheckOutOfScope { get; private init; }

    public NotificationRule ToEntity()
    {
        return new NotificationRule()
        {
            Name = ShortName.Create(Name),
            Subject = AssetClass.Create(Subject),
            Topic = Topic,
            Template = Template,
            CheckOutOfScope = CheckOutOfScope,
            Filter = Filter
        };
    }
}
