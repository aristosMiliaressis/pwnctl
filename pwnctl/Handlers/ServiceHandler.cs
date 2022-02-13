using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pwnctl.Entities;
using pwnctl.Persistence;
using pwnctl.Services;

namespace pwnctl.Handlers
{
    public class ServiceHandler : IAssetHandler<Service>
    {
        private readonly JobQueueService _queueService = new();
        private readonly PwntainerDbContext _context = new();

        public ServiceHandler() {}

        public async Task HandleAsync(IAsset asset)
        {
            var service = asset as Service;

            var existingService = _context.Services
                                                .Include(s => s.Host)
                                                .Include(s => s.Domain)
                                                .AsEnumerable()
                                                .FirstOrDefault(s => s.Origin == service.Origin);
            if (existingService != null)
            {
                return;
            }

            if (service.Host != null)
            {
                var host = await _context.Hosts.FirstOrDefaultAsync(h => h.IP == service.Host.IP);
                if (host == null)
                {
                    _context.Hosts.Add(service.Host);
                }
                else
                {
                    service.Host = host;
                }
            }
            if (service.Domain != null)
            {
                var domain = await _context.Domains.FirstOrDefaultAsync(h => h.Name == service.Domain.Name);
                if (domain == null)
                {
                    _context.Domains.Add(service.Domain);
                }
                else
                {
                    service.Domain = domain;
                }
            }

            _context.Services.Add(service);

            await _context.SaveChangesAsync();
        }
    }
}