namespace pwnctl.app.Notifications.Interfaces;

using pwnctl.app.Notifications.Entities;
using pwnctl.domain.BaseClasses;

public interface NotificationRepository
{
    List<NotificationRule> ListRules();

    Task<Notification> FindNotificationAsync(Asset asset, NotificationRule rule);
}