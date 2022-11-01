
using Microsoft.AspNetCore.Mvc.Routing;
using pwnctl.dto.Mediator;

namespace pwnctl.api.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class HttpEndpointAttribute<TRequest> : HttpMethodAttribute
        where TRequest : IBaseMediatedRequest
    {
        public HttpEndpointAttribute()
            : base(new List<string>{ GetMethod().Method }, GetRoute())
        {

        }

        private static string GetRoute()
        {
            return string.Join("/", typeof(TRequest).GetProperty(nameof(IBaseMediatedRequest.Route)).GetValue(null).ToString().Split("/").Skip(1));
        }

        private static HttpMethod GetMethod()
        {
            return (HttpMethod)typeof(TRequest).GetProperty(nameof(IBaseMediatedRequest.Method)).GetValue(null);
        }
    }
}