using pwnctl.worker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<JobConsumerService>();

pwnctl.infra.Configuration.ConfigurationManager.Load();

var app = builder.Build();

app.Run();