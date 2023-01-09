namespace pwnctl.api.Middleware;

using pwnctl.app;
using pwnctl.app.Common.Exceptions;
using pwnctl.api.Extensions;
using pwnctl.dto.Mediator;
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
        catch (AppException ex)
        {
            var responseModel = MediatedResponse.Error(ex.Message);

            await context.Response.Create(responseModel);
        }
        catch (Exception ex)
        {
            PwnInfraContext.Logger.Exception(ex);

            await context.Response.Create(HttpStatusCode.InternalServerError);
        }
    }
}
