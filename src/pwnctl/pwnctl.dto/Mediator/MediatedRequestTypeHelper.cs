namespace pwnctl.dto.Mediator;

public static class MediatedRequestTypeHelper
{
    public static string GetRoute(Type type)
    {
        return string.Join("/", type.GetProperty(nameof(IBaseMediatedRequest.Route)).GetValue(null).ToString().Split("/").Skip(2));
    }

    public static HttpMethod GetMethod(Type type)
    {
        return (HttpMethod)type.GetProperty(nameof(IBaseMediatedRequest.Method)).GetValue(null);
    }

    public static void CheckRequestConfig(Type type)
    {
        throw new NotImplementedException();
    }
}