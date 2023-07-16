using pwnctl.infra.DependencyInjection;
using pwnctl.proc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<OutputProcessorService>();

await PwnInfraContextInitializer.SetupAsync();

var app = builder.Build();

app.Run();
