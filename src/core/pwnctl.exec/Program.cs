using pwnctl.infra.DependencyInjection;
using pwnctl.exec;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<TaskExecutorService>();

await PwnInfraContextInitializer.SetupAsync();

var app = builder.Build();

app.Run();
