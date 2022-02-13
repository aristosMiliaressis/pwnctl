using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pwnctl.Entities;
using pwnctl.Persistence;
using pwnctl.Services;

namespace pwnctl.Handlers
{
    public class VirtualHostHandler : IAssetHandler<VirtualHost>
    {
        private readonly JobQueueService _queueService = new();
        private readonly PwntainerDbContext _context = new();

        public VirtualHostHandler() {}

        public async Task HandleAsync(IAsset asset)
        {
            throw new NotImplementedException();
        }
    }
}