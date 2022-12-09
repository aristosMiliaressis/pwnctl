namespace pwnctl.app.Interfaces;

using pwnctl.domain.BaseClasses;
using pwnctl.app.Entities;

public interface NotificationSender
{
    static NotificationSender Instance { get; set; }

    void Send(Asset asset, NotificationRule rule);
}