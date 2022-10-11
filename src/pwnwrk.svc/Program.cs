using pwnwrk.infra;
using pwnwrk.svc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<JobConsumerService>();

await PwnwrkInfraFacade.SetupAsync();

var app = builder.Build();

app.Run();