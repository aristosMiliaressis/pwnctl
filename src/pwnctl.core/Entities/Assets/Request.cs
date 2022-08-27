using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using pwnctl.core.Entities.Assets;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities.Assets
{
    public class Request : BaseAsset
    {
        public int EndpointId { get; set; }
        public Endpoint Endpoint { get; set; }

        public int? ResponseId { get; set; }
        public Response Response { get; set; }

        public string Method { get; set; }
        //public Dictionary<string, string> Headers { get; set; }

        public List<Parameter> Parameters { get; set; }
        public string Body { get; set; }

        private Request() {}
        
        public Request(Endpoint endpoint, string method, string queryString, 
                    Dictionary<string, string> headers, string body)
        {
            Endpoint = endpoint;
            Method = method;
            //QueryString = queryString;
            //Headers = headers;
            Body = body;
        }

        public static bool TryParse(string assetText, List<Tag> tags, out BaseAsset[] assets)
        {
            throw new NotImplementedException();
        }

        public override bool Matches(ScopeDefinition definition)
        {
            throw new NotImplementedException();
        }
    }
}
