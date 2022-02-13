using pwnctl;
using pwnctl.Persistence;
using pwnctl.Entities;
using pwnctl.Handlers;
using pwnctl.Parsers;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace pwnctl.Services
{
    public class ScopeService
    {
        private readonly PwntainerDbContext _context = new();

        private static ScopeService _instance;
  
        public static ScopeService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ScopeService();
    
                return _instance;
            }
        }

        private ScopeService() { }

        public bool IsInScope(IAsset asset)
        {
            if (typeof(Domain).IsAssignableFrom(asset.GetType()))
            {
                var domain = asset as Domain;
                var scopeDefinitions = _context.ScopeDefinitions.Where(d => d.Type == ScopeDefinition.ScopeType.DomainRegex).ToList();
                return scopeDefinitions.Select(d => new Regex(d.Pattern)).Any(regex => regex.Matches(domain.Name).Count > 0);
            }
            else if (typeof(Endpoint).IsAssignableFrom(asset.GetType()))
            {
                var endpoint = asset as Endpoint;
                var scopeDefinitions = _context.ScopeDefinitions.Where(d => d.Type == ScopeDefinition.ScopeType.UrlRegex).ToList();
                return scopeDefinitions.Select(d => new Regex(d.Pattern)).Any(regex => regex.Matches(endpoint.Uri).Count > 0);
            }
            else if (typeof(Host).IsAssignableFrom(asset.GetType()))
            {
                var host = asset as Host;
                var scopeDefinitions = _context.ScopeDefinitions.Where(d => d.Type == ScopeDefinition.ScopeType.CIDR).ToList();
                return scopeDefinitions.Any(d => IsInCIDRRange(host.IP, d.Pattern));
            }

            return false;
        }

        private bool IsInCIDRRange(string ipAddress, string CIDRmask)
        {
            string[] parts = CIDRmask.Split('/');

            int IP_addr = BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), 0);
            int CIDR_addr = BitConverter.ToInt32(IPAddress.Parse(parts[0]).GetAddressBytes(), 0);
            int CIDR_mask = IPAddress.HostToNetworkOrder(-1 << (32 - int.Parse(parts[1])));

            return ((IP_addr & CIDR_mask) == (CIDR_addr & CIDR_mask));
        }
    }
}
