using pwnctl.Parsers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace pwnctl.Entities
{
    public class Response : BaseAsset, IAsset
    {
        public int RequestId { get; set; }
        public Request Request { get; set; }

        public string Status { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string BodyHash { get; set; }
        public string TimeMs { get; set; }
        public string Title { get; set; }

        private Response() {}
        
        public Response(Request request, string status, Dictionary<string, string> headers, 
                        string hash = null, string time = null, string title = null)
        {
            Request = request;
            Status = status;
            Headers = headers;
            TimeMs = hash;
            Title = title;
        }

        public static bool TryParse(string assetText, out Response response)
        {
            throw new NotImplementedException();
        }
    }
}
