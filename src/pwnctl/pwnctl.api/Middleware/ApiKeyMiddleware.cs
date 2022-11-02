namespace pwnctl.api.Middleware;

using pwnctl.api.Extensions;
using pwnwrk.infra;
using System.Net;

public sealed class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string _apiKeyHeader = "X-Api-Key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(_apiKeyHeader, out var extractedApiKey)
         || !PwnContext.Config.Api.ApiKey.Equals(extractedApiKey))
        {
            await context.Response.Create(HttpStatusCode.Unauthorized);

            return;
        }

        await _next(context);
    }
}
