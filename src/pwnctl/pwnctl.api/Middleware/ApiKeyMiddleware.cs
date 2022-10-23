namespace pwnctl.api.Middleware;

using pwnctl.api.Models;
using pwnctl.api.Extensions;
using System.Text.Json;
using System.Net;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string _apiKeyHeader = "X-Api-Key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
        var apiKey = appSettings.GetValue<string>(_apiKeyHeader);

        if (!context.Request.Headers.TryGetValue(_apiKeyHeader, out var extractedApiKey)
            || !apiKey.Equals(extractedApiKey))
        {
            await context.Response.Create(HttpStatusCode.Unauthorized, ApiResponse.Unauthorized);

            return;
        }

        await _next(context);
    }
}