using System;
using System.Collections.Generic;
using System.Text;
using pwnctl.core.Entities.Assets;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities.Assets
{
    public class Parameter : BaseAsset
    {
        private Parameter() {}
        
        public Parameter(Endpoint endpoint, string name, ParamType type, string urlEncodedCsValues)
        {
            Endpoint = endpoint;
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
            throw new NotImplementedException();
        }

        public Endpoint Endpoint { get; set; }
        public int EndpointId { get; set; }

        public string Name { get; set; }
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
    }
}
