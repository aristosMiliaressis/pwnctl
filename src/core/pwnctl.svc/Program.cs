using pwnctl.infra;
using pwnctl.svc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<TaskConsumerService>();

PwnContext.Setup();

var app = builder.Build();

app.Run();