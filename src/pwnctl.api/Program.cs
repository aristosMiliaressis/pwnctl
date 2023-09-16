using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using pwnctl.app;
using pwnctl.app.Users.Entities;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.api;
using pwnctl.api.Extensions;
using pwnctl.api.Middleware;
using pwnctl.api.Mediator.Pipelines;
using pwnctl.dto;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using pwnctl.infra.Repositories;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.infra.Queueing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddControllers();

builder.Services.AddMediatR(typeof(PwnctlDtoAssemblyMarker), typeof(Program));

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuditLoggingPipeline<,>));

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));

builder.Services.AddValidatorsFromAssemblyContaining<PwnctlDtoAssemblyMarker>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<PwnctlDbContext>();

builder.Services.AddIdentity<User, IdentityRole>()
      .AddEntityFrameworkStores<PwnctlDbContext>();

builder.Services.AddTransient<TaskQueueService, SQSTaskQueueService>();

builder.Services.AddTransient<UserManager<User>>();

builder.Services.AddTransient<SignInManager<User>>();

builder.Services.AddTransient<BearerTokenManager>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwt =>
    {
        jwt.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(PwnInfraContext.Config.Api.HMACSecret)),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ValidateAudience = false,
            ValidateIssuer = false
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<PwnctlDbContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 10;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = true;
});

var app = builder.Build();

var userManager = app.Services.GetService<UserManager<User>>();

PwnInfraContextInitializer.Setup();

await DatabaseInitializer.InitializeAsync(userManager);

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapMediatedEndpoints(typeof(PwnctlDtoAssemblyMarker));

app.MapControllers();

app.Run();
