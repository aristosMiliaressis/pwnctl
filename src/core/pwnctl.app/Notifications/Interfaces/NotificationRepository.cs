namespace pwnctl.app.Notifications.Interfaces;

using pwnctl.app.Notifications.Entities;
using pwnctl.domain.BaseClasses;

public interface NotificationRepository
{
    IEnumerable<NotificationRule> ListRules();

    Task<Notification> FindNotificationAsync(Asset asset, NotificationRule rule);
}