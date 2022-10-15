using pwnwrk.domain.Attributes;
using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities.Assets
{
    public class Parameter : BaseAsset
    {
        public Endpoint Endpoint { get; private init; }
        public string EndpointId { get; private init; }
        [UniquenessAttribute]
        public string Url { get; private init; }

        [UniquenessAttribute]
        public string Name { get; private init; }
        [UniquenessAttribute]
        public ParamType Type { get; private init; }

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

        public static bool TryParse(string assetText, List<Tag> tags, out BaseAsset[] assets)
        {
            throw new NotImplementedException();
        }

        public override bool Matches(ScopeDefinition definition)
        {
            return Endpoint.Matches(definition);
        }

        public enum ParamType
        {
            Query,
            Body,
            Path, // if segment is integer or guid or md5 or email
            Cookie,
            Header,
        }

        public override string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}
