namespace pwnctl.api.Middleware;

using pwnwrk.infra;
using pwnwrk.infra.Logging;
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
            PwnContext.Logger.Error(ex.ToRecursiveExInfo());

            await context.Response.Create(HttpStatusCode.InternalServerError);

            return;
        }
    }
}
