using pwnctl.Parsers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace pwnctl.Entities
{
    public class Request : BaseAsset, IAsset
    {
        public int EndpointId { get; set; }
        public Endpoint Endpoint { get; set; }

        public int? ResponseId { get; set; }
        public Response Response { get; set; }

        public string Method { get; set; }
        public string QueryString { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }

        private Request() {}
        
        public Request(Endpoint endpoint, string method, string queryString, 
                    Dictionary<string, string> headers, string body)
        {
            Endpoint = endpoint;
            Method = method;
            QueryString = queryString;
            Headers = headers;
            Body = body;
        }

        public static bool TryParse(string assetText, out Request request)
        {
            throw new NotImplementedException();
        }
    }
}
