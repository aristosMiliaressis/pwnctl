namespace pwnctl.domain.Entities;

using pwnctl.kernel.Attributes;
using pwnctl.kernel.BaseClasses;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

public sealed class HttpParameter : Asset
{
    public HttpEndpoint? Endpoint { get; private init; }
    public Guid? EndpointId { get; private init; }
    [EqualityComponent]
    public string Url { get; init; }

    [EqualityComponent]
    public string Name { get; init; }
    [EqualityComponent]
    public ParamType Type { get; init; }

    public string? Value { get; private init; }

    public HttpParameter() {}
    
    public HttpParameter(HttpEndpoint endpoint, string name, ParamType type, string? value)
    {
        Endpoint = endpoint;
        Url = endpoint.ToString();
        Name = name;
        Type = type;
        Value = value;
    }

    public static Result<HttpParameter, string> TryParse(string assetText)
    {
        try
        {
            return $"{assetText} is not a {nameof(HttpParameter)}";
        }
        catch
        {
            return $"{assetText} is not a {nameof(HttpParameter)}";
        }
    }

    public override string ToString()
    {
        return Type switch
        {
            ParamType.Query => Url+"?"+Name+"=",
            _ => throw new NotImplementedException()
        };
    }
}