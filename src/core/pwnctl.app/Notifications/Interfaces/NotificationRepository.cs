namespace pwnctl.app.Notifications.Interfaces;

using pwnctl.app.Notifications.Entities;

public interface NotificationRepository
{
    List<NotificationRule> ListRules();
}