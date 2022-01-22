using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pwnctl.Entities;
using pwnctl.DataEF;
using pwnctl.Services;

namespace pwnctl.Handlers
{
    public class NetRangeHandler : IAssetHandler<NetRange>
    {
        private readonly JobQueueService _queueService = new();
        private readonly PwntainerDbContext _context = new();

        public NetRangeHandler() {}

        public async Task HandleAsync(IAsset asset)
        {
            var netRange = asset as NetRange;

            var existingNetRange = await _context.NetRanges.FirstOrDefaultAsync(e => e.CIDR == netRange.CIDR);
            if (existingNetRange != null)
            {
                return;
            }

            _context.NetRanges.Add(netRange);

            await _context.SaveChangesAsync();
        }
    }
}