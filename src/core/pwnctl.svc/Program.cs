using pwnctl.infra;
using pwnctl.infra.Configuration;
using pwnctl.infra.Notifications;
using pwnctl.svc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<TaskConsumerService>();

PwnContext.Setup();

var app = builder.Build();

var notificationSender = new DiscordNotificationSender();;

notificationSender.Send($"pwnctl service started on {EnvironmentVariables.HOSTNAME}", "status");

app.Run();

notificationSender.Send($"pwnctl service stoped on {EnvironmentVariables.HOSTNAME}", "status");
