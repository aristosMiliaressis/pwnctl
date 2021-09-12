using DnsClient;
using Pwntainer.Application.Entities;
using Pwntainer.Application.Wrappers;
using Pwntainer.Core;
using Pwntainer.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Pwntainer.Persistence.Services
{
    public class AssetService
    {
        private readonly PwntainerDbContext _context;
        private readonly LookupClient _client = new LookupClient();

        public AssetService(PwntainerDbContext context)
        {
            _context = context;
        }

        public void AddAsset(string asset)
        {
            asset = asset.Split("?")[0];

            if (AssetInspector.IsUrl(asset))
            {
                var scheme = asset.Split("://")[0];
                var uri = asset.Split("://")[1];
                var host = uri.Split("/")[0];

                string path = "/";
                if (uri.Contains("/"))
                    path += uri.Split("/")[1];

                var hostDto = HandleHostSegment(host, scheme);
                var service = HandleService(hostDto);

                _context.SaveChanges();

                var existingEndpoint = _context.Endpoints.FirstOrDefault(e => e.Uri == $"{scheme}://{host}{path}");
                if (existingEndpoint == null)
                {
                    var endpoint = new Endpoint();
                    endpoint.Uri = $"{scheme}://{host}{path}";
                    endpoint.ServiceId = service.Id;
                    _context.Endpoints.Add(endpoint);
                }
            }
            else if (AssetInspector.IsIp(asset) || AssetInspector.IsDomain(asset))
            {

                var hostDto = HandleHostSegment(asset);
                
                HandleService(hostDto);
            }
            else if (AssetInspector.IsWildcardDomain(asset))
            {
                HandleWildcardDomain(asset);
            }
            else if (AssetInspector.IsCidr(asset))
            {
                HandleCIDR(asset);
            }

            _context.SaveChanges();
        }

        private HostDto HandleHostSegment(string host, string scheme = null)
        { 
            var hostDto = new HostDto();

            if (host.Contains("*"))
            {
                HandleWildcardDomain(host);
            }
            else
            {
                if (host.Contains(":"))
                {
                    hostDto.Port = ushort.Parse(host.Split(":").Last());
                    host = host.Split(":")[0];
                    if (AssetInspector.IsIp(host))
                    {
                        hostDto.Ip = host;
                        var existingHost = _context.Hosts.FirstOrDefault(r => r.IP == hostDto.Ip);
                        if (existingHost == null)
                        {
                            var hostIp = new Host() { IP = hostDto.Ip };
                            _context.Hosts.Add(hostIp);
                        }
                    }
                    else
                    {
                        var existingDomain = _context.Domains.FirstOrDefault(e => e.Name == host);
                        if (existingDomain == null)
                        {
                            var domain = new Domain();
                            domain.Name = host;
                            _context.Domains.Add(domain);
                        }

                        hostDto.Domain = host;
                        foreach (var aRecord in _client.Query(host, QueryType.A).Answers.ARecords())
                        {
                            hostDto.Ip = aRecord.Address.ToString();

                            var existingHost = _context.Hosts.FirstOrDefault(r => r.IP == hostDto.Ip);
                            if (existingHost == null)
                            {
                                var hostIp = new Host() { IP = hostDto.Ip };
                                _context.Hosts.Add(hostIp);
                            }

                            var existingRecord = _context.ARecords.FirstOrDefault(r => r.DomainName == hostDto.Domain && r.IP == hostDto.Ip);
                            if (existingRecord == null)
                            {
                                var record = new ARecord();
                                record.DomainName = hostDto.Domain;
                                record.IP = hostDto.Ip;
                                _context.ARecords.Add(record);
                            }
                        }
                    }
                }
                else
                {
                    if (scheme != null)
                    {
                        hostDto.Port = UriSchemeToPortMap[scheme];
                    }

                    if (AssetInspector.IsIp(host))
                    {
                        hostDto.Ip = host;
                        var existingHost = _context.Hosts.FirstOrDefault(r => r.IP == hostDto.Ip);
                        if (existingHost == null)
                        {
                            var hostIp = new Host() { IP = hostDto.Ip };
                            _context.Hosts.Add(hostIp);
                        }
                    }
                    else
                    {
                        var existingDomain = _context.Domains.FirstOrDefault(e => e.Name == host);
                        if (existingDomain == null)
                        {
                            var domain = new Domain();
                            domain.Name = host;
                            _context.Domains.Add(domain);
                        }

                        hostDto.Domain = host;
                        foreach (var aRecord in _client.Query(host, QueryType.A).Answers.ARecords())
                        {
                            hostDto.Ip = aRecord.Address.ToString();

                            var existingHost = _context.Hosts.FirstOrDefault(r => r.IP == hostDto.Ip);
                            if (existingHost == null)
                            {
                                var hostIp = new Host() { IP = hostDto.Ip };
                                _context.Hosts.Add(hostIp);
                            }

                            var existingRecord = _context.ARecords.FirstOrDefault(r => r.DomainName == hostDto.Domain && r.IP == hostDto.Ip);
                            if (existingRecord == null)
                            {
                                var record = new ARecord();
                                record.DomainName = hostDto.Domain;
                                record.IP = hostDto.Ip;
                                _context.ARecords.Add(record);
                            }
                        }
                    }
                }
            }

            // NmapWrapper.Run("-O", "80,443", host.Ip);

            return hostDto;
        }

        private Service HandleService(HostDto host)
        {
            var existingService = _context.Services.FirstOrDefault(e => e.IP == host.Ip & e.Port == host.Port);
            if (existingService == null)
            {
                if (host.Port == 0)
                {
                    //var nmapResult = NmapWrapper.Run("-sS", "-p", "80,443", host.Ip);
                    //if (nmapResult.Any(p => p.Port == 443 & p.Status == NmapWrapper.NmapOutputEntry.PortStatus.Open))
                    //    host.Port = 443;
                    //else if (nmapResult.Any(p => p.Port == 80 & p.Status == NmapWrapper.NmapOutputEntry.PortStatus.Open))
                    //    host.Port = 80;
                    //else
                    //    return null;
                }

                var service = new Service();
                service.IP = host.Ip;
                service.Port = host.Port;
                service.TransportProtocol = TransportProtocol.TCP;

                //var ports = NmapWrapper.Run("-sSV", "-p", service.Port.ToString(), service.IP);
                //service.Protocol = ports.First().Protocol;

                //var serviceTags = AnalyzeServiceBanner(ports.First().Banner);

                //using (var httpClient = new HttpClient())
                //{
                //    var response = httpClient.GetAsync($"{service.Protocol}://{service.IP}:{service.Port}").Result;

                //    var httpTags = AnalyzeHttpHeaders(response.Headers);
                //}

                _context.Services.Add(service);
                existingService = service;
            }

            return existingService;
        }

        private ServiceTag[] AnalyzeHttpHeaders(HttpHeaders headers)
        {
            var tags = new ServiceTag[] { };

            // TODO: add tags for http headers

            if (headers.TryGetValues("Server", out IEnumerable<string> serverHeader))
            {
            }
            if (headers.TryGetValues("X-Powered-By", out IEnumerable<string> poweredByHeader))
            {
                //https://webtechsurvey.com/response-header/x-powered-by
            }
            if (headers.TryGetValues("X-AspNet-Version", out IEnumerable<string> aspNetVersionHeader))
            {
            }

            // nginx headers, CORS headers
            //X-Cache, X-Cache-Hits, X-Cache-Miss, X-Served-By
            //Authorization, WWW-Authenticate
            //Proxy-Authenticate, Proxy-Authorization, Proxy-Connection

            return tags;
        }

        private ServiceTag[] AnalyzeServiceBanner(string banner)
        {
            var tags = new ServiceTag[] { };

            // TODO: add tag for banner

            if (banner.Contains("ssh", StringComparison.OrdinalIgnoreCase))
            {
                // OpenSSH
            }
            else if (banner.Contains("ftp", StringComparison.OrdinalIgnoreCase))
            {

            }
            else
            { 
            }

            return tags;
        }

        private void HandleWildcardDomain(string domain)
        {
            if (!domain.Split("*.").Last().Contains("*"))
            {
                var newDomain = new Domain() { Name = domain.Split("*.").Last() };
                _context.Domains.Add(newDomain);
            }

            var wildcard = new WildcardDomain() { Pattern = domain };
            _context.WildcardDomains.Add(wildcard);
        }

        private bool IsHttpServer(string host, ushort port = 443, bool useTLS = true)
        {
            throw new NotImplementedException();
        }

        private List<string> GetDomainHierarchy(string domain)
        {
            var domains = new List<string>();

            var parts = domain.Split(".").Reverse().ToArray();

            if (!AssetInspector.IsTld(parts[0]))
                return null;

            var temp = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                temp = parts[i] + "." + temp;
                domains.Add(temp);
            }

            return domains;
        }

        private void HandleCIDR(string cidr)
        {
            var netRange = new NetRange() { CIDR = cidr };

            _context.NetRanges.Add(netRange);
        }

        private readonly Dictionary<string, ushort> UriSchemeToPortMap = new Dictionary<string, ushort>
        {
            { "https", 443 },
            { "http(s)", 443 },
            { "http", 80 },
            { "ftp", 21 },
            { "ssh", 22 }
        };

        private readonly Dictionary<ushort, string> PortToProtoMap = new Dictionary<ushort, string>
        {
            { 21, "ftp" },
            { 22, "ssh" },
            { 25, "smtp" },
            { 80, "http" },
            { 443, "http" },
            { 8443, "http" }
        };

        public class HostDto
        {
            public string Ip { get; set; }
            public string Domain { get; set; }
            public ushort Port { get; set; }
        }
    }
}
