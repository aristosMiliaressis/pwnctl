using pwnctl.infra.DependencyInjection;
using pwnctl.exec;
using pwnctl.infra.Commands;
using pwnctl.infra.Configuration;
using pwnctl.infra.Queueing;
using pwnctl.infra.Notifications;
using pwnctl.app.Common.Interfaces;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Notifications.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<TaskExecutorService>();

PwnInfraContextInitializer.Setup();
PwnInfraContextInitializer.Register<TaskQueueService, SQSTaskQueueService>();
if (EnvironmentVariables.IN_VPC)
{
    PwnInfraContextInitializer.Register<NotificationSender, DiscordNotificationSender>();
    PwnInfraContextInitializer.Register<CommandExecutor, BashCommandExecutor>();
}
else
{
    PwnInfraContextInitializer.Register<NotificationSender, StubNotificationSender>();
    PwnInfraContextInitializer.Register<CommandExecutor, StubCommandExecutor>();
}

var app = builder.Build();

app.Run();
