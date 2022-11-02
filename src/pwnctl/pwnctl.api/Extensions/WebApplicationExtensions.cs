namespace pwnctl.api.Extensions;

using System.Reflection;
using pwnctl.dto.Mediator;

public static class WebApplicationExtensions
{
    public static void MapMediatedEndpoints(this WebApplication app, params Type[] requestAssemblyMarkerTypes)
    {
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

            // TODO: create request delegate
            // var requestDelegate = new RequestDelegate();

            // app.MapMethods(routePattern, new List<string> { requestMethod.Method }, requestDelegate);
        }

        throw new NotImplementedException();
    }
}
