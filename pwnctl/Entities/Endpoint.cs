using pwnctl.Parsers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace pwnctl.Entities
{
    public class Endpoint : BaseAsset, IAsset
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public string Uri { get; set; }
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

        private Endpoint() {}
        
        public Endpoint(string scheme, Service service, string path)
        {
            Scheme = scheme;
            Service = service;
            Path = path;
            Uri = $"{scheme}://{Service.Origin}{path}" + (path.EndsWith("/") ? "" : "/");
        }

        public static bool TryParse(string assetText, out Endpoint endpoint)
        {
            try
            {
                var uri = new Uri(assetText);

                var origin = Host.TryParse(uri.Host, out Host host)
                        ? new Service(null, host, (ushort)uri.Port)
                        : new Service(new Domain(uri.Host), null, (ushort)uri.Port);

                endpoint = new Endpoint(uri.Scheme, origin, uri.AbsolutePath);
                return true;
            }
            catch
            {
                endpoint = null;
                return false;
            }
        }
    }
}
