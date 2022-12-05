namespace pwnwrk.domain.Notifications.Interfaces;

using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Notifications.Entities;

public interface NotificationSender
{
    static NotificationSender Instance { get; set; }

    void Send(Asset asset, NotificationRule rule);
}