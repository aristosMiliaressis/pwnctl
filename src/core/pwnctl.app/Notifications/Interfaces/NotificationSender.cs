namespace pwnctl.app.Notifications.Interfaces;

using pwnctl.domain.BaseClasses;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Notifications.Enums;

public interface NotificationSender
{
    void Send(Asset asset, NotificationRule rule);

    void Send(string message, NotificationTopic topic);
}