using pwnctl.infra.DependencyInjection;
using pwnctl.proc;
using pwnctl.infra.Configuration;
using pwnctl.infra.Queueing;
using pwnctl.infra.Notifications;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Notifications.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<OutputProcessorService>();

PwnInfraContextInitializer.Setup();
PwnInfraContextInitializer.Register<TaskQueueService, SQSTaskQueueService>();
if (EnvironmentVariables.IN_VPC)
{
    PwnInfraContextInitializer.Register<NotificationSender, DiscordNotificationSender>();
}
else
{
    PwnInfraContextInitializer.Register<NotificationSender, StubNotificationSender>();
}

var app = builder.Build();

app.Run();
