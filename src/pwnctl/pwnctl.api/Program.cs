using pwnctl.api.Middleware;
using pwnctl.api.MediatR.Pipelines;
using pwnctl.dto;
using MediatR;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddMediatR(typeof(PwnctlDtoAssemblyMarker), typeof(Program));

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuditLoggingPipeline<,>));

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));

builder.Services.AddValidatorsFromAssemblyContaining<PwnctlDtoAssemblyMarker>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();
