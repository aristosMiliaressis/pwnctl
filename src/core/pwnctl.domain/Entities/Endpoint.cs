using pwnctl.domain.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;

namespace pwnctl.domain.Entities
{
    public sealed class Endpoint : Asset
    {
        [UniquenessAttribute]
        public string Url { get; init; }

        public string ServiceId { get; private init; }
        public Service Service { get; init; }

        public string Scheme { get; init; }
        public string Path { get; init; }
        public string Filename 
        {
            get
            {
                var parts = Path.Split("/").Last().Split(".");
                return parts.Length > 1  && parts.Last().Length > 1 ? parts.Last() : null;
            }
        }

        public string Extension
        {
            get
            {
                var parts = Path.Split(".");
                if (parts.Count() == 1)
                    return string.Empty;

                return parts.Last().Length > 4
                    ? string.Empty
                    : parts.Last();
            }
        }

        public Endpoint() {}
        
        public Endpoint(string scheme, Service service, string path)
        {
            Scheme = scheme;
            Service = service;
            Path = string.IsNullOrEmpty(path) ? "/" : path;
            Url = $"{Service.Origin.Replace("tcp", scheme)}{path}" + (path.EndsWith("/") ? "" : "/");
        }

        public static bool Parse(string assetText, out Asset[] assets)
        {
            var _assets = new List<Asset>();

            var uri = new Uri(assetText);

            var origin = Host.Parse(uri.Host, out Asset[] hostAssets)
                    ? new Service((Host)hostAssets[0], (ushort)uri.Port)
                    : new Service(new Domain(uri.Host), (ushort)uri.Port);

            var endpoint = new Endpoint(uri.Scheme, origin, uri.AbsolutePath);

            _assets.Add(endpoint);
            _assets.Add(origin);
            if (origin.Domain != null) _assets.Add(origin.Domain);
            if (origin.Host != null) _assets.Add(origin.Host);

            var _params = uri.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped)
                .Split("&")
                .Select(p => new Parameter(endpoint, p.Split("=")[0], ParamType.Query, null))
                .Where(p => !string.IsNullOrEmpty(p.Name));

            // Adds all subdirectories
            string path = endpoint.Path;
            do
            {
                path = string.Join("/", path.Split("/").Reverse().Skip(1).Reverse());
                _assets.Add(new Endpoint(endpoint.Scheme, endpoint.Service, path));
            } while(path.Length > 1);

            if (_params.Any())
                _assets.AddRange(_params);

            assets = _assets.ToArray();
            return true;
        }

        public override string ToString()
        {
            return Url;
        }
    }
}
