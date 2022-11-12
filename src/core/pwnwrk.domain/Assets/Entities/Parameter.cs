using pwnwrk.domain.Assets.Attributes;
using pwnwrk.domain.Assets.DTO;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Common.Entities;
using pwnwrk.domain.Targets.Entities;

namespace pwnwrk.domain.Assets.Entities
{
    public sealed class Parameter : Asset
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

        public static bool TryParse(string assetText, List<Tag> tags, out Asset[] assets)
        {
            throw new NotImplementedException();
        }

        internal override bool Matches(ScopeDefinition definition)
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

        public override AssetDTO ToDTO()
        {
            throw new NotImplementedException();
        }
    }
}
