using pwnctl.core.Attributes;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities.Assets
{
    public class Parameter : BaseAsset
    {
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

        public Endpoint Endpoint { get; set; }
        public string EndpointId { get; set; }
        [UniquenessAttribute]
        public string Url { get; set; }

        [UniquenessAttribute]
        public string Name { get; set; }
        [UniquenessAttribute]
        public ParamType Type { get; set; }

        public string UrlEncodedCsValues { get; set; }

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
