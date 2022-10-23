using pwnwrk.svc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<JobConsumerService>();

var app = builder.Build();

app.Run();