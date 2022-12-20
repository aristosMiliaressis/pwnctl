using pwnctl.domain.BaseClasses;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Notifications.Interfaces;
using System.Diagnostics;

namespace pwnctl.infra.Notifications
{
    public sealed class DiscordNotificationSender : NotificationSender
    {
        public void Send(Asset asset, NotificationRule rule)
        {
            Send($"{asset.DomainIdentifier} triggered rule {rule.ShortName}", rule.Topic);
        }

        public void Send(string message, string topic)
        {
            CommandExecutor.ExecuteAsync("/root/go/bin/notify", $"-bulk -provider discord -id {topic}", message).Wait();
        }
    }
}
