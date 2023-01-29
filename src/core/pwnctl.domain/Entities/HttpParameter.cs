using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

namespace pwnctl.domain.Entities
{
    public sealed class HttpParameter : Asset
    {
        public HttpEndpoint Endpoint { get; private init; }
        public string EndpointId { get; private init; }
        [EqualityComponent]
        public string Url { get; init; } // TODO : 

        [EqualityComponent]
        public string Name { get; init; }
        [EqualityComponent]
        public ParamType Type { get; init; }

        public string UrlEncodedCsValues { get; private init; }

        public HttpParameter() {}
        
        public HttpParameter(HttpEndpoint endpoint, string name, ParamType type, string urlEncodedCsValues)
        {
            Endpoint = endpoint;
            Url = endpoint.Url;
            Name = name;
            Type = type;
            UrlEncodedCsValues = urlEncodedCsValues;
        }

        public static bool TryParse(string assetText, out Asset asset)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Type switch
            {
                ParamType.Query => $"{Url}?{Name}=",
                _ => throw new NotImplementedException()
            };
        }
    }
}