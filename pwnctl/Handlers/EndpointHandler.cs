using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pwnctl.Entities;
using pwnctl.DataEF;
using pwnctl.Services;

namespace pwnctl.Handlers
{
    public class EndpointHandler : IAssetHandler<Endpoint>
    {
        private readonly JobQueueService _queueService = new();
        private readonly PwntainerDbContext _context = new();

        public EndpointHandler() {}

        public async Task HandleAsync(IAsset asset)
        {
            var endpoint = asset as Endpoint;

            var existingEndpoint = await _context.Endpoints.FirstOrDefaultAsync(e => e.Uri == endpoint.Uri);
            if (existingEndpoint != null)
            {
                return;
            }

            var origin = _context.Services
                                    .Include(s => s.Host)
                                        .ThenInclude(h => h.AARecords)
                                        .ThenInclude(h => h.Domain)
                                    .Include(s => s.Domain)
                                    .AsEnumerable()
                                    .FirstOrDefault(s => s.Origin == endpoint.Service.Origin);
            if (origin == null)
            {
                origin = endpoint.Service;
                _context.Services.Add(origin);
            }

            endpoint.Service = origin;
            _context.Endpoints.Add(endpoint);

            // foreach (var record in endpoint.Service.Host.AARecords)
            // {
            //     // TODO: test if vhost alreadt exists
            //     var vhost = new VirtualHost(endpoint.Service, record.Domain.Name);
                
            //     _context.VirtualHosts.Add(vhost);
            // }

            if (endpoint.InScope)
            {
                if (endpoint.Path == "/")
                {
                    _queueService.Enqueue($"ffuf {endpoint.Uri} /opt/pwntainer/wordlists/SecLists/Discovery/Web-Content/common.txt");
                }

                _queueService.Enqueue($"crawl {endpoint.Uri}");
            }

            await _context.SaveChangesAsync();
        }
    }
}