namespace pwnctl.dto.Mediator;

public static class MediatedRequestTypeHelper
{
    public static string GetRoute(Type type)
    {
        return string.Join("/", type.GetProperty(nameof(IBaseMediatedRequest.Route)).GetValue(null).ToString().Split("/"));
    }

    public static HttpMethod GetVerb(Type type)
    {
        return (HttpMethod)type.GetProperty(nameof(IBaseMediatedRequest.Verb)).GetValue(null);
    }

    public static void CheckRequestConfig(Type type)
    {
        throw new NotImplementedException();
    }
}