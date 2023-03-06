using pwnctl.infra.DependencyInjection;
using pwnctl.svc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<TaskConsumerService>();

PwnInfraContextInitializer.Setup();

var app = builder.Build();

app.Run();
