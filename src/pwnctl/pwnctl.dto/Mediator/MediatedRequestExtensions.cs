namespace pwnctl.dto.Mediator;

public static class MediatedRequestExtensions
{
    public static void CheckRequestConfig<TRequest>()
    {
        // TODO: check at startup
        // Method & route not null
        // route contains only valid chars
        // route args exist on model
        throw new NotImplementedException();
    }

    public static string GetInterpolatedRoute(this IBaseMediatedRequest request)
    {
        string route = request.ReflectedConcreteRoute;
        int idx = 0;

        request.ReflectedConcreteRoute.Split("{")
            .Skip(1)
            .ToList()
            .ForEach(arg =>
            {
                arg = arg.Split("}")[0];
                var routeParam = request.GetType().GetProperty(arg).GetValue(request).ToString();

                route = route.Replace("{" + arg + "}", routeParam);
                idx++;
            });

        return route;
    }
}
