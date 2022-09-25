using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using pwnctl.worker;
using pwnctl.infra.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<JobConsumerService>();

var app = builder.Build();

app.Run();