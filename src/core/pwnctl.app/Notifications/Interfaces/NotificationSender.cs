namespace pwnctl.app.Notifications.Interfaces;

using pwnctl.domain.BaseClasses;
using pwnctl.app.Notifications.Entities;

public interface NotificationSender
{
    void Send(Asset asset, NotificationRule rule);

    void Send(string message, string topic);
}