using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pwnctl.Entities;
using pwnctl.DataEF;
using pwnctl.Services;

namespace pwnctl.Handlers
{
    public class HostHandler : IAssetHandler<Host>
    {
        private readonly JobQueueService _queueService = new();
        private readonly PwntainerDbContext _context = new();

        public HostHandler() {}

        public async Task HandleAsync(IAsset asset)
        {
            var host = asset as Host;

            var existingHost = await _context.Hosts.FirstOrDefaultAsync(e => e.IP == host.IP);
            if (existingHost != null)
            {
                return;
            }

            _context.Hosts.Add(host);

            _queueService.Enqueue($"dig +short -x {host.IP} | pwnctl");
            _queueService.Enqueue($"echo {host.IP} | httpx -silent | pwnctl");
            _queueService.Enqueue($"portscan {host.IP}");
            
            await _context.SaveChangesAsync();
        }
    }
}