using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pwnctl.Entities;
using pwnctl.DataEF;
using pwnctl.Services;

namespace pwnctl.Handlers
{
    public class ParameterHandler : IAssetHandler<Parameter>
    {
        private readonly JobQueueService _queueService = new();
        private readonly PwntainerDbContext _context = new();

        public ParameterHandler() {}

        public async Task HandleAsync(IAsset asset)
        {
            throw new NotImplementedException();
        }
    }
}