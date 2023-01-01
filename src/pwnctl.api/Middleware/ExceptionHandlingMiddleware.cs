namespace pwnctl.api.Middleware;

using pwnctl.app;
using pwnctl.api.Extensions;
using System.Net;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            PwnInfraContext.Logger.Exception(ex);

            await context.Response.Create(HttpStatusCode.InternalServerError);

            return;
        }
    }
}
