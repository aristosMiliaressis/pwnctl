using System.Text.RegularExpressions;
using pwnwrk.domain.Assets.Attributes;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.DTO;
using pwnwrk.domain.Assets.Enums;
using pwnwrk.domain.Common.Entities;
using pwnwrk.domain.Targets.Entities;
using pwnwrk.domain.Targets.Enums;

namespace pwnwrk.domain.Assets.Entities
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

        public static bool TryParse(string assetText, List<Tag> tags, out Asset[] assets)
        {
            var _assets = new List<Asset>();
            try
            {
                var uri = new Uri(assetText);

                var origin = Host.TryParse(uri.Host, null, out Asset[] hostAssets)
                        ? new Service((Host)hostAssets[0], (ushort)uri.Port)
                        : new Service(new Domain(uri.Host), (ushort)uri.Port);

                var endpoint = new Endpoint(uri.Scheme, origin, uri.AbsolutePath);
                endpoint.AddTags(tags);

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
            catch
            {
                assets = null;
                return false;
            }
        }

        internal override bool Matches(ScopeDefinition definition)
        {
            return Service.Matches(definition)
                || (definition.Type == ScopeType.UrlRegex && new Regex(definition.Pattern).Matches(Url).Count > 0);
        }

        public override AssetDTO ToDTO()
        {
            var dto = new AssetDTO
            {
                Asset = Url,
                Tags = new Dictionary<string, object>
                {
                    {"Extension", Extension},
                    {"Filename", Filename},
                    {"Path", Path},
                    {"Origin", Service.Origin},
                    {"Port", Service.Port.ToString()},
                    {"Host", Service?.Host?.IP},
                    {"Domain", Service?.Domain?.Name},
                    {"Scheme", Scheme}
                },
                Metadata = new Dictionary<string, string>
                {
                    {"InScope", InScope.ToString()},
                    {"FoundAt", FoundAt.ToString()},
                    {"FoundBy", FoundBy }
                }
            };

            Tags.ForEach(t => dto.Tags.Add(t.Name, t.Value));

            return dto;
        }
    }
}
