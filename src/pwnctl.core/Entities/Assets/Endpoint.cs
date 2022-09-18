using pwnctl.core.Attributes;
using pwnctl.core.BaseClasses;
using pwnctl.core.Models;
using Newtonsoft.Json;

namespace pwnctl.core.Entities.Assets
{
    public class Endpoint : BaseAsset
    {
        [UniquenessAttribute]
        public string Url { get; set; }

        public string ServiceId { get; set; }
        public Service Service { get; set; }

        public string Scheme { get; set; }
        public string Path { get; set; }
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

        private Endpoint() {}
        
        public Endpoint(string scheme, Service service, string path)
        {
            Scheme = scheme;
            Service = service;
            Path = path;
            Url = $"{Service.Origin.Replace("tcp", scheme)}{path}" + (path.EndsWith("/") ? "" : "/");
        }

        public static bool TryParse(string assetText, List<Tag> tags, out BaseAsset[] assets)
        {
            var _assets = new List<BaseAsset>();
            try
            {
                var uri = new Uri(assetText);

                var origin = Host.TryParse(uri.Host, null, out BaseAsset[] hostAssets)
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
                    .Select(p => new Parameter(endpoint, p.Split("=")[0], Parameter.ParamType.Query, null))
                    .Where(p => !string.IsNullOrEmpty(p.Name));
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

        public override bool Matches(ScopeDefinition definition)
        {
            return Service.Matches(definition);
        }

        public override string ToJson()
        {
            var dto = new AssetDTO
            {
                Asset = Url,
                Tags = new Dictionary<string, string>
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
                    {"FoundBy", FoundBy.ToString()}
                }
            };

            Tags.ForEach(t => dto.Tags.Add(t.Name, t.Value));

            return JsonConvert.SerializeObject(dto);
        }
    }
}
