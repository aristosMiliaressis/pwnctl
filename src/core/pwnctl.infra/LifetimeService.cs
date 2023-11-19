using Microsoft.Extensions.Hosting;
using pwnctl.app;
using pwnctl.app.Notifications.Enums;
using pwnctl.infra.Configuration;

namespace pwnctl.infra;

public abstract class LifetimeService : BackgroundService
{
    private readonly IHostApplicationLifetime _svcLifetime;

    public LifetimeService(IHostApplicationLifetime svcLifetime)
    {
        _svcLifetime = svcLifetime;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await PwnInfraContext.NotificationSender.SendAsync($"{GetType().Name} started.", NotificationTopic.Status);

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await PwnInfraContext.NotificationSender.SendAsync($"{GetType().Name} caught scale in event, exiting gracefully.", NotificationTopic.Status);
        
        await base.StopAsync(cancellationToken);

        _svcLifetime.StopApplication();
    }
}