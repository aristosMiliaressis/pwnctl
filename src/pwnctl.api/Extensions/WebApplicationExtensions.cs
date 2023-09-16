namespace pwnctl.api.Extensions;

using System.Reflection;
using MediatR;
using pwnctl.dto.Mediator;
using pwnctl.app;
using pwnctl.app.Common.Extensions;
using System.ComponentModel;

public static class WebApplicationExtensions
{
    /// <summary>
    /// an extension method to map the `MediatedEndpoint` infrastructure.
    /// a custom implementation to centralize the api contract into a single assembly consumed by both the producer & consumer of the api
    /// </summary>
    /// <notice>
    /// currently only supports JSON based REST APIs and no fancy request filters.
    /// </notice>
    public static void MapMediatedEndpoints(this WebApplication app, params Type[] requestAssemblyMarkerTypes)
    {
        if (!requestAssemblyMarkerTypes.Any())
            throw new ArgumentException("Atleast one assembly marker type is required", nameof(requestAssemblyMarkerTypes));

        var assemblies = requestAssemblyMarkerTypes.Select(type => Assembly.GetAssembly(type));

        var requestInterfaceType = typeof(Request);

        var requestTypes = assemblies
                            .SelectMany(assembly => assembly
                                .GetTypes()
                                .Where(type => !type.IsInterface
                                            && type.IsAssignableTo(requestInterfaceType)));

        foreach (var requestType in requestTypes)
        {
            var routePattern = MediatedRequestTypeHelper.GetRoute(requestType);

            routePattern.ValidateTemplate(requestType);

            var requestVerb = MediatedRequestTypeHelper.GetVerb(requestType);

            if (requestVerb is null)
            {
                throw new Exception($"Mediated Request {requestType.Name} method is required but set to null.");
            }

            var mapMethod = typeof(EndpointRouteBuilderExtensions)
                                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                                .Where(m => m.Name.ToUpper() == ("MAP"+requestVerb.Method))
                                .Where(m => m.GetParameters()
                                        .Select(p => p.ParameterType)
                                        .Contains(typeof(RequestDelegate)))
                                .First();

            RequestDelegate requestDelegate = async context =>
            {
                string json = null;
                using (var sr = new StreamReader(context.Request.Body))
                {
                    json = await sr.ReadToEndAsync();
                }

                if (string.IsNullOrEmpty(json)) json = "{}";

                var mediator = context.RequestServices.GetService<IMediator>();

                var request = PwnInfraContext.Serializer.Deserialize(json, requestType);

                request = context.Request.Path.Value.Extrapolate(routePattern, request);
                foreach (var param in context.Request.Query)
                {
                    var prop = requestType.GetProperty(param.Key);
                    if (prop is not null)
                        prop.SetValue(request, Convert.ChangeType(param.Value.ToString(), prop.PropertyType));
                }

                var result = (MediatedResponse)await mediator.Send(request);

                await context.Response.Create(result);
            };

            mapMethod.Invoke(null, new object[] { app, routePattern, requestDelegate });
        }
    }
}
