using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

namespace pwnctl.domain.Entities
{
    public sealed class HttpEndpoint : Asset
    {
        [EqualityComponent]
        public string Url { get; init; }

        public Guid SocketAddressId { get; private init; }
        public NetworkSocket Socket { get; init; }
        public Guid? ParentEndpointId { get; private init; }
        public HttpEndpoint ParentEndpoint { get; private set; }
        public List<HttpParameter> HttpParameters { get; private set; }

        public string Scheme { get; init; }
        public string Path { get; init; }

        public HttpEndpoint() {}

        public HttpEndpoint(string scheme, NetworkSocket address, string path)
        {
            Scheme = scheme;
            Socket = address;
            Path = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;
            Path = string.IsNullOrEmpty(Path) ? "/" : Path;

            string hostSegment = Socket.NetworkHost != null ? Socket.NetworkHost.IP : Socket.DomainName.Name;
            string portSegment = (scheme == "http" && Socket.Port == 80) || (scheme == "https" && Socket.Port == 443) ? "" : (":" + Socket.Port);
            Url = Scheme+"://"+hostSegment+portSegment+Path;
        }

        public static HttpEndpoint TryParse(string assetText)
        {
            if (!(assetText.ToLower().StartsWith("http") && assetText.Contains("://"))
                && !assetText.StartsWith("//"))
                return null;

            var uri = new Uri(assetText);

            // if url is protocol relative, treat it as an https url
            string scheme = uri.Port == -1 ? "https" : uri.Scheme;
            ushort port = (ushort) (uri.Port == -1 ? 443 : uri.Port);

            var host = NetworkHost.TryParse(uri.Host);
            var socket = host != null
                    ? new NetworkSocket(host, port)
                    : new NetworkSocket(new DomainName(uri.Host), port);

            var endpoint = new HttpEndpoint(scheme, socket, uri.AbsolutePath);

            var _params = uri.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped)
                            .Split("&")
                            .Select(p => p.Split("=")[0])
                            .Distinct()
                            .Select(p => new HttpParameter(endpoint, p, ParamType.Query, null))
                            .Where(p => !string.IsNullOrEmpty(p.Name))
                            .ToList();

            endpoint.HttpParameters = _params;

            var furthestEndpoint = endpoint;

            // Adds all subdirectories
            string path = endpoint.Path;
            do
            {
                path = string.Join("/", path.Split("/").Reverse().Skip(1).Reverse());
                endpoint.ParentEndpoint = new HttpEndpoint(endpoint.Scheme, endpoint.Socket, path);
                endpoint = endpoint.ParentEndpoint;
            } while (path.Length > 1);

            return furthestEndpoint;
        }

        public override string ToString()
        {
            return Url;
        }
    }
}
