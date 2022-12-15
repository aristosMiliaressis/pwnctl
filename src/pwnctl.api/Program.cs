using pwnctl.api.Extensions;
using pwnctl.api.Middleware;
using pwnctl.api.Mediator.Pipelines;
using pwnctl.dto;
using MediatR;
using FluentValidation;
using pwnctl.infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddControllers();

builder.Services.AddMediatR(typeof(PwnctlDtoAssemblyMarker), typeof(Program));

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuditLoggingPipeline<,>));

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));

builder.Services.AddValidatorsFromAssemblyContaining<PwnctlDtoAssemblyMarker>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapMediatedEndpoints(typeof(PwnctlDtoAssemblyMarker));

app.MapControllers();

PwnContext.Setup();

app.Run();
