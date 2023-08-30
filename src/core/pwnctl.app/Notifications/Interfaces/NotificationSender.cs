namespace pwnctl.app.Notifications.Interfaces;

using pwnctl.app.Notifications.Entities;
using pwnctl.app.Notifications.Enums;

public interface NotificationSender
{
    Task SendAsync(Notification notification);

    Task SendAsync(string message, NotificationTopic topic);
}
