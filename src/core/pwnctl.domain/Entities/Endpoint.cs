using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

namespace pwnctl.domain.Entities
{
    public sealed class Endpoint : Asset
    {
        [EqualityComponent]
        public string Url { get; init; }

        public string ServiceId { get; private init; }
        public Service Service { get; init; }
        public string ParentEndpointId { get; private init; }
        public Endpoint ParentEndpoint { get; private set; }
        public List<Parameter> Parameters { get; private set; }

        public string Scheme { get; init; }
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

        public Endpoint() {}
        
        public Endpoint(string scheme, Service service, string path)
        {
            Scheme = scheme;
            Service = service;
            Path = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;
            Path = string.IsNullOrEmpty(Path) ? "/" : Path;
            
            string hostSegment = Service.Host != null ? Service.Host.IP : Service.Domain.Name;
            string portSegment = (scheme == "http" && Service.Port == 80) || (scheme == "https" && Service.Port == 443) ? "" : (":" + Service.Port);
            Url = $"{Scheme}://{hostSegment}{portSegment}{Path}";
        }

        public static bool TryParse(string assetText, out Asset asset)
        {
            asset = null;

            if (!assetText.Contains("://") || !assetText.ToLower().StartsWith("http"))
                return false;

            var uri = new Uri(assetText);

            var origin = Host.TryParse(uri.Host, out Asset host)
                    ? new Service((Host)host, (ushort)uri.Port)
                    : new Service(new Domain(uri.Host), (ushort)uri.Port);

            var endpoint = new Endpoint(uri.Scheme, origin, uri.AbsolutePath);

            var _params = uri.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped)
                .Split("&")
                .Select(p => new Parameter(endpoint, p.Split("=")[0], ParamType.Query, null))
                .Where(p => !string.IsNullOrEmpty(p.Name))
                .ToList();

            endpoint.Parameters = _params;
            asset = endpoint;

            // Adds all subdirectories
            string path = endpoint.Path;
            do
            {
                path = string.Join("/", path.Split("/").Reverse().Skip(1).Reverse());
                endpoint.ParentEndpoint = new Endpoint(endpoint.Scheme, endpoint.Service, path);
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
