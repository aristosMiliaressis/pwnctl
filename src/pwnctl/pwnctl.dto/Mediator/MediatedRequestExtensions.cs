namespace pwnctl.dto.Mediator;

public static class MediatedRequestExtensions
{
    public static string GetInterpolatedRoute(this IBaseMediatedRequest request)
    {
        string route = MediatedRequestTypeHelper.GetRoute(request.GetType());

        route.Split("{")
            .Skip(1)
            .ToList()
            .ForEach(arg =>
            {
                arg = arg.Split("}")[0];
                var routeParam = request.GetType().GetProperty(arg).GetValue(request).ToString();

                route = route.Replace("{" + arg + "}", routeParam);
            });

        return route;
    }
}
