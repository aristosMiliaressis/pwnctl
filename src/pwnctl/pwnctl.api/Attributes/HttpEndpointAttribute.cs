
using Microsoft.AspNetCore.Mvc.Routing;
using pwnctl.dto.Mediator;
using pwnctl.api.Extensions;

namespace pwnctl.api.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class HttpEndpointAttribute<TRequest> : HttpMethodAttribute
        where TRequest : IBaseMediatedRequest
    {
        public HttpEndpointAttribute()
            : base(new List<string>{ MediatedRequestTypeHelper.GetMethod(typeof(TRequest)).Method }, 
                                    MediatedRequestTypeHelper.GetRoute(typeof(TRequest)))
        {

        }

    }
}