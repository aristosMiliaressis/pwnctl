using pwnwrk.svc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<JobConsumerService>();

pwnwrk.infra.Configuration.ConfigurationManager.Load();

var app = builder.Build();

app.Run();