using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Notifications;
using pwnctl.svc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<TaskConsumerService>();

PwnInfraContextInitializer.Setup();

var app = builder.Build();

var notificationSender = new DiscordNotificationSender();;

app.Run();
