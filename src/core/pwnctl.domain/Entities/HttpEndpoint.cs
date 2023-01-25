using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

namespace pwnctl.domain.Entities
{
    public sealed class HttpEndpoint : Asset
    {
        [EqualityComponent]
        public string Url { get; init; }

        public string SocketAddressId { get; private init; }
        public NetworkSocket Socket { get; init; }
        public string ParentEndpointId { get; private init; }
        public HttpEndpoint ParentEndpoint { get; private set; }
        public List<HttpParameter> HttpParameters { get; private set; }

        public string Scheme { get; init; } // TODO validate
        public string Path { get; init; }
        public string Filename 
        {
            get
            {
                if (Path == null)
                    return null;
                    
                var lastPart = Path.Split("/").Where(p => !string.IsNullOrEmpty(p)).LastOrDefault(); 
                return lastPart != null && lastPart.Contains(".") ? lastPart : null;
            }
        }

        public string Extension => Filename == null ? null : Filename.Split(".").Last();

        public HttpEndpoint() {}
        
        public HttpEndpoint(string scheme, NetworkSocket address, string path)
        {
            Scheme = scheme;
            Socket = address;
            Path = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;
            Path = string.IsNullOrEmpty(Path) ? "/" : Path;
            
            string hostSegment = Socket.NetworkHost != null ? Socket.NetworkHost.IP : Socket.DomainName.Name;
            string portSegment = (scheme == "http" && Socket.Port == 80) || (scheme == "https" && Socket.Port == 443) ? "" : (":" + Socket.Port);
            Url = $"{Scheme}://{hostSegment}{portSegment}{Path}";
        }

        public static bool TryParse(string assetText, out Asset asset)
        {
            asset = null;

            if (!assetText.Contains("://") || !assetText.ToLower().StartsWith("http"))
                return false;

            var uri = new Uri(assetText);

            var address = NetworkHost.TryParse(uri.Host, out Asset host)
                    ? new NetworkSocket((NetworkHost)host, (ushort)uri.Port)
                    : new NetworkSocket(new DomainName(uri.Host), (ushort)uri.Port);

            var endpoint = new HttpEndpoint(uri.Scheme, address, uri.AbsolutePath);

            var _params = uri.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped)
                .Split("&")
                .Select(p => new HttpParameter(endpoint, p.Split("=")[0], ParamType.Query, null))
                .Where(p => !string.IsNullOrEmpty(p.Name))
                .ToList();

            endpoint.HttpParameters = _params;
            asset = endpoint;

            // Adds all subdirectories
            string path = endpoint.Path;
            do
            {
                path = string.Join("/", path.Split("/").Reverse().Skip(1).Reverse());
                endpoint.ParentEndpoint = new HttpEndpoint(endpoint.Scheme, endpoint.Socket, path);
                endpoint = endpoint.ParentEndpoint;
            } while (path.Length > 1);

            return true;
        }

        public override string ToString()
        {
            return Url;
        }
    }
}
