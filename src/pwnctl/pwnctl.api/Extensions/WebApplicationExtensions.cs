namespace pwnctl.api.Extensions;

using System.Text.Json;
using System.Reflection;
using MediatR;
using pwnctl.dto.Mediator;

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

        var requestInterfaceType = typeof(IBaseMediatedRequest);

        var requestTypes = assemblies
                            .SelectMany(assembly => assembly
                                .GetTypes()
                                .Where(type => !type.IsInterface
                                            && type.IsAssignableTo(requestInterfaceType)));
        
        foreach (var requestType in requestTypes)
        {
            var routePattern = MediatedRequestTypeHelper.GetRoute(requestType);

            var requestMethod = MediatedRequestTypeHelper.GetMethod(requestType);

            if (requestMethod == null)
            {
                throw new Exception($"Mediated Request {requestType.Name} method is required but set to null.");
            }

            // TODO: validate route syntax & args

            app.MapMethods(routePattern, 
                        new List<string> { requestMethod.Method }, 
                        async context => 
            {
                IBaseMediatedRequest request = null;
                using (var sr = new StreamReader(context.Request.Body))
                {
                    var json = await sr.ReadToEndAsync();
                    request = JsonSerializer.Deserialize<IBaseMediatedRequest>(json);
                }

                var mediator = context.RequestServices.GetService<IMediator>();

                var result = (MediatedResponse) await mediator.Send(request);

                await context.Response.Create(result);
            });
        }
    }
}
