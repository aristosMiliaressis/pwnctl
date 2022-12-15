using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

namespace pwnctl.domain.Entities
{
    public sealed class Parameter : Asset
    {
        public Endpoint Endpoint { get; private init; }
        public string EndpointId { get; private init; }
        [EqualityComponent]
        public string Url { get; init; }

        [EqualityComponent]
        public string Name { get; init; }
        [EqualityComponent]
        public ParamType Type { get; init; }

        public string UrlEncodedCsValues { get; private init; }

        private Parameter() {}
        
        public Parameter(Endpoint endpoint, string name, ParamType type, string urlEncodedCsValues)
        {
            Endpoint = endpoint;
            Url = endpoint.Url;
            Name = name;
            Type = type;
            UrlEncodedCsValues = urlEncodedCsValues;
        }

        public static bool TryParse(string assetText, out Asset mainAsset, out Asset[] relatedAssets)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
