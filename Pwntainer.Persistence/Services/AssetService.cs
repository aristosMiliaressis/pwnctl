using DnsClient;
using Microsoft.EntityFrameworkCore;
using Pwntainer.Application.Entities;
using Pwntainer.Application.Wrappers;
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
            if (string.IsNullOrEmpty(asset))
                return;

            asset = asset.Split("?")[0];
            
            if (Endpoint.IsUrl(asset))
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
                if (existingEndpoint == null && service != null)
                {
                    var endpoint = new Endpoint();
                    endpoint.Uri = $"{scheme}://{host}{path}";
                    endpoint.ServiceId = service.Id;
                    _context.Endpoints.Add(endpoint);
                }
            }
            else if (Host.IsIp(asset) || Domain.IsDomain(asset))
            {

                var hostDto = HandleHostSegment(asset);
                
                HandleService(hostDto);
            }
            else if (Domain.IsWildcardDomain(asset))
            {
                HandleWildcardDomain(asset);
            }
            else if (NetRange.IsCidr(asset))
            {
                HandleCIDR(asset);
            }
            else if (DNSRecord.IsDNSRecord(asset))
            {
                HandleDNSRecord(asset);
            }

            _context.SaveChanges();
        }

        private void HandleDNSRecord(string asset)
        {
            var record = new DNSRecord();
            var parts = asset.Replace("\t", " ").Split(" ");
            switch (parts[2])
            {
                case "A":
                    { 
                    record.Type = RecordType.A;
                    record.Value = parts[3];
                    
                    var existingHost = _context.Hosts.FirstOrDefault(r => r.IP == parts[3]);
                    if (existingHost == null)
                    {
                        var host = HandleHostSegment(parts[3]);
                        existingHost = _context.Hosts.FirstOrDefault(r => r.IP == parts[3]);
                    }

                    var existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                    if (existingDomain == null)
                    {
                        HandleHostSegment(parts[0]);
                        existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                    }

                    record.Host = existingHost;
                    record.Domain = existingDomain;
                    break;
                    }
                case "AAAA":
                    {
                        record.Type = RecordType.AAAA;
                        record.Value = parts[3];

                        var existingHost = _context.Hosts.FirstOrDefault(r => r.IP == parts[3]);
                        if (existingHost == null)
                        {
                            var host = HandleHostSegment(parts[3]);
                            existingHost = _context.Hosts.FirstOrDefault(r => r.IP == parts[3]);
                        }

                        var existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        if (existingDomain == null)
                        {
                            HandleHostSegment(parts[0]);
                            existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        }

                        record.Host = existingHost;
                        record.Domain = existingDomain;
                        break;
                    }
                case "CNAME":
                    {
                        record.Type = RecordType.CNAME;
                        record.Value = parts[3];

                        var existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        if (existingDomain == null)
                        {
                            HandleHostSegment(parts[0]);
                            existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        }

                        record.Domain = existingDomain;
                        break;
                    }
                case "NS":
                    {
                        record.Type = RecordType.NS;
                        record.Value = parts[3];

                        var existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        if (existingDomain == null)
                        {
                            HandleHostSegment(parts[0]);
                            existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        }

                        record.Domain = existingDomain;
                        break;
                    }
                case "MX":
                    {
                        record.Type = RecordType.MX;
                        record.Value = string.Join(" ", parts[2..]);

                        var existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        if (existingDomain == null)
                        {
                            HandleHostSegment(parts[0]);
                            existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        }

                        record.Domain = existingDomain;
                        break;
                    }
                case "PTR":
                    {
                        record.Type = RecordType.PTR;
                        record.Value = parts[3];

                        var existingHost = _context.Hosts.FirstOrDefault(r => r.IP == parts[3]);
                        if (existingHost == null)
                        {
                            var host = HandleHostSegment(parts[3]);
                            existingHost = _context.Hosts.FirstOrDefault(r => r.IP == parts[3]);
                        }

                        record.Host = existingHost;
                        break;
                    }
                case "TXT":
                    {
                        record.Type = RecordType.TXT;
                        record.Value = parts[3];

                        var existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        if (existingDomain == null)
                        {
                            HandleHostSegment(parts[0]);
                            existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        }

                        record.Domain = existingDomain;
                        break;
                    }
                case "SOA":
                    {
                        record.Type = RecordType.SOA;
                        record.Value = string.Join(" ", parts[2..]);

                        var existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        if (existingDomain == null)
                        {
                            HandleHostSegment(parts[0]);
                            existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        }

                        record.Domain = existingDomain;
                        break;
                    }
                case "SRV":
                    {
                        record.Type = RecordType.SRV;
                        record.Value = string.Join(" ", parts[2..]);

                        var existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        if (existingDomain == null)
                        {
                            HandleHostSegment(parts[0]);
                            existingDomain = _context.Domains.FirstOrDefault(r => r.Name == parts[0]);
                        }

                        record.Domain = existingDomain;
                        break;
                    }
            }

            _context.Add(record);
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
                    if (Host.IsIp(host))
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

                            existingDomain = domain;
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

                                existingHost = hostIp;
                            }

                            var existingRecord = _context.DNSRecords
                                                        .Include(a => a.Domain)
                                                        .Include(a => a.Host)
                                                        .FirstOrDefault(r => r.Domain.Name == hostDto.Domain && r.Host.IP == hostDto.Ip);
                            if (existingRecord == null)
                            {
                                var record = new DNSRecord
                                {
                                    Type = RecordType.A,
                                    Domain = existingDomain,
                                    Host = existingHost,
                                    Value = hostDto.Ip
                                };
                                _context.DNSRecords.Add(record);
                            }
                            _context.SaveChanges();
                        }
                    }
                }
                else
                {
                    if (scheme != null)
                    {
                        hostDto.Port = UriSchemeToPortMap[scheme];
                    }

                    if (Host.IsIp(host))
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

                            existingDomain = domain;
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

                                existingHost = hostIp;
                            }

                            var existingRecord = _context.DNSRecords
                                                        .Include(a => a.Domain)
                                                        .Include(a => a.Host)
                                                        .FirstOrDefault(r => r.Domain.Name == hostDto.Domain && r.Host.IP == hostDto.Ip);
                            if (existingRecord == null)
                            {
                                var record = new DNSRecord
                                {
                                    Domain = existingDomain,
                                    Host = existingHost
                                };
                                _context.DNSRecords.Add(record);
                            }
                            _context.SaveChanges();
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
                    return null;
                }

                if (host.Ip == null)
                    return null;

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

            if (!Domain.IsTld(parts[0]))
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
