
using Microsoft.AspNetCore.Mvc.Routing;
using pwnctl.dto;

namespace pwnctl.api.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class HttpEndpointAttribute : HttpMethodAttribute
    {
        public HttpEndpointAttribute(Type type)
            : this((HttpMethod)type.GetProperty(nameof(IApiRequest.Method)).GetValue(null), 
            string.Join("/", type.GetProperty(nameof(IApiRequest.Route)).GetValue(null).ToString().Split("/").Skip(1)))
        {

        }

        public HttpEndpointAttribute(HttpMethod method, string template)
            : base(new List<string>{ method.Method }, template)
        {

        }
    }
}