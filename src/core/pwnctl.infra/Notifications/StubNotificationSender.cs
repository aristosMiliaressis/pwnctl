using pwnctl.domain.BaseClasses;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.app.Notifications.Enums;

namespace pwnctl.infra.Notifications
{
    public sealed class StubNotificationSender : NotificationSender
    {
        public void Send(Notification notification)
        {
        }

        public void Send(string message, NotificationTopic topic)
        {
        }
    }
}