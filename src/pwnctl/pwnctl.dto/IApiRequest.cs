namespace pwnctl.dto;

public interface IApiRequest
{
    static abstract string Route { get; }
    static abstract HttpMethod Method { get; }
    string ReflectedConcreteRoute => (string)GetType().GetProperty(nameof(IApiRequest.Route)).GetValue(null);
    HttpMethod ReflectedConcreteMethod => (HttpMethod)GetType().GetProperty(nameof(IApiRequest.Method)).GetValue(null);
}

public interface IApiRequest<TResponse> : IApiRequest
{

}
