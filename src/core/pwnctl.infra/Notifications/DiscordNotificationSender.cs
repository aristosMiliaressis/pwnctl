using pwnctl.domain.BaseClasses;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.infra.Commands;
using pwnctl.app.Notifications.Enums;

namespace pwnctl.infra.Notifications
{
    public sealed class DiscordNotificationSender : NotificationSender
    {
        public void Send(Notification notification)
        {
            Send(notification.GetText(), notification.Rule.Topic);
        }

        public void Send(string message, NotificationTopic topic)
        {
            CommandExecutor.ExecuteAsync($"echo {message} | /root/go/bin/notify -bulk -provider discord -id {topic.ToString().ToLower()}").Wait();
        }
    }
}
