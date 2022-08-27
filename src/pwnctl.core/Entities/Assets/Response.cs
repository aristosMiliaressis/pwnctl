using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities.Assets
{
    public class Response : BaseAsset
    {
        public int RequestId { get; set; }
        public Request Request { get; set; }

        public string Status { get; set; }
        //public Dictionary<string, string> Headers { get; set; }
        public string BodyHash { get; set; }
        public string TimeMs { get; set; }
        public string Title { get; set; }

        private Response() {}
        
        public Response(Request request, string status, Dictionary<string, string> headers, 
                        string hash = null, string time = null, string title = null)
        {
            Request = request;
            Status = status;
            //Headers = headers;
            TimeMs = hash;
            Title = title;
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
