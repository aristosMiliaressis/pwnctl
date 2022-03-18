using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pwnctl.Entities;
using pwnctl.Persistence;
using pwnctl.Services;

namespace pwnctl.Handlers
{
    public class DNSRecordHandler : IAssetHandler<DNSRecord>
    {
        private readonly JobQueueService _queueService = new();
        private readonly PwntainerDbContext _context = new();

        public async Task HandleAsync(IAsset asset)
        {
            var record = asset as DNSRecord;

            var existingRecord = await _context.DNSRecords.FirstOrDefaultAsync(e => e.Key == record.Key && e.Type == record.Type);
            if (existingRecord != null)
            {
                return;
            }

            if (record.Host != null)
            {
                var host = await _context.Hosts.FirstOrDefaultAsync(h => h.IP == record.Host.IP);
                if (host == null)
                {
                    _context.Hosts.Add(record.Host);
                }
                else
                {
                    record.Host = host;
                }
            }
            if (record.Domain != null)
            {
                var domain = await _context.Domains.FirstOrDefaultAsync(h => h.Name == record.Domain.Name);
                if (domain == null)
                {
                    _context.Domains.Add(record.Domain);
                }
                else
                {
                    record.Domain = domain;
                }
            }

            _context.DNSRecords.Add(record);

            await _context.SaveChangesAsync();
        }
    }
}