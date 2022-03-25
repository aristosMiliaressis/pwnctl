using System;
using System.Collections.Generic;
using System.Text;

namespace pwnctl.Entities
{
    public class Parameter : BaseAsset, IAsset
    {
        private Parameter() {}
        
        public Parameter(Endpoint endpoint, string name, ParamType type, string urlEncodedCsValues)
        {
            Endpoint = endpoint;
            Name = name;
            Type = type;
            UrlEncodedCsValues = urlEncodedCsValues;
        }

        public static bool TryParse(string assetText, out Parameter parameter)
        {
            throw new NotImplementedException();
        }

        public Endpoint Endpoint { get; set; }
        public int EndpointId { get; set; }

        public Request Request { get; set; }
        public int? RequestId { get; set; }

        public string Name { get; set; }
        public ParamType Type { get; set; }

        public string UrlEncodedCsValues { get; set; }

        public enum ParamType
        {
            Query,
            Body,
            Cookie,
            Header,
        }
    }
}
