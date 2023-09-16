using pwnctl.domain.BaseClasses;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.infra.Commands;
using pwnctl.app.Notifications.Enums;
using pwnctl.app;

namespace pwnctl.infra.Notifications
{
    public sealed class DiscordNotificationSender : NotificationSender
    {
        public async Task SendAsync(Notification notification)
        {
            await SendAsync(notification.GetText(), notification.Rule.Topic);
        }

        public async Task SendAsync(string message, NotificationTopic topic)
        {
            await PwnInfraContext.CommandExecutor.ExecuteAsync($"echo {message} | /root/go/bin/notify -bulk -provider discord -id {topic.ToString().ToLower()}");
        }
    }
}
