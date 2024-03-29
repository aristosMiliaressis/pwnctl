namespace pwnctl.infra.Notifications;

using pwnctl.domain.BaseClasses;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.app.Notifications.Enums;
using pwnctl.app;

public sealed class StubNotificationSender : NotificationSender
{
    public async Task SendAsync(Notification notification)
    {
        await SendAsync(notification.GetText(), notification.Rule!.Topic);
    }

    public Task SendAsync(string message, NotificationTopic topic)
    {
        PwnInfraContext.Logger.Information("Notification: " + message);

        return Task.CompletedTask;
    }
}