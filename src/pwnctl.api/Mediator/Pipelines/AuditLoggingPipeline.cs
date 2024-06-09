using MediatR;
using pwnctl.app;
using pwnctl.dto.Mediator;

namespace pwnctl.api.Mediator.Pipelines
{
    public sealed class AuditLoggingPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
            where TResponse : MediatedResponse
            where TRequest : IRequest<TResponse>
    {
        // private readonly IHttpContextAccessor _httpContextAccessor;

        // public AuditLoggingPipeline(IHttpContextAccessor httpContextAccessor)
        // {
        //     _httpContextAccessor = httpContextAccessor;
        // }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            //var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;

            //PwnInfraContext.Logger.Information($"{ip} requested {typeof(TRequest).Name}");
            PwnInfraContext.Logger.Information($"Executing {typeof(TRequest).Name}");

            return await next();
        }
    }
}