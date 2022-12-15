using pwnctl.svc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<TaskConsumerService>();

var app = builder.Build();

app.Run();