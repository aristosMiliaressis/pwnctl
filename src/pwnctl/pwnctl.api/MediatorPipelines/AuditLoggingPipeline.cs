using MediatR;
using pwnwrk.infra;
using pwnwrk.infra.MediatR;
using Microsoft.AspNetCore.Http;

namespace pwnctl.api.MediatorPipelines
{
    public class AuditLoggingPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
            where TResponse : MediatorResponse
            where TRequest : IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLoggingPipeline(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;

            PwnContext.Logger.Information("${ip} requested {TRequest.GetType().Name}");

            return await next();

            // TODO: debug level logging of request/response objects
        }
    }
}