using pwnctl.worker;
using pwnctl.infra.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<JobConsumerService>();

pwnctl.infra.Configuration.ConfigurationManager.Load();

var app = builder.Build();

app.Run();