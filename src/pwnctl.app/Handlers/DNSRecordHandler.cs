using Microsoft.EntityFrameworkCore;
using pwnctl.app.Utilities;
using pwnctl.core.BaseClasses;
using pwnctl.core.Entities.Assets;

namespace pwnctl.app.Handlers
{
    public class DNSRecordHandler : BaseAssetHandler<DNSRecord>
    {
        private readonly JobAssignmentService _jobService = new();

        protected override async Task<DNSRecord> HandleAsync(DNSRecord record)
        {
            if (record.Host != null)
            {
                record.Host  = await _context.Hosts
                                            .Include(h => h.AARecords)
                                            .ThenInclude(r => r.Domain)
                                            .FirstOrDefaultAsync(d => d.IP == record.Host.IP);
            }

            if (!ScopeChecker.Singleton.IsInScope(record))
                return record;

            if (record.Type != DNSRecord.RecordType.A && record.Type != DNSRecord.RecordType.AAAA)
                return record;

            var domain = await _context.Domains.FirstOrDefaultAsync(d => d.Name == record.Key);
            var host = await _context.Hosts.FirstOrDefaultAsync(d => d.IP == record.Value);

            if (domain != null && !domain.InScope)
            {
                domain.InScope = true;
                await _context.SaveChangesAsync();
                _jobService.Assign(domain);
            }

            if (host != null && !host.InScope)
            {
                // this prevents some db errors
                _context.ChangeTracker.Entries()
                                        .Where(e => e.Entity.GetType() == typeof(Domain))
                                        .ToList()
                                        .ForEach(d => d.State = EntityState.Detached);
                host.InScope = true;
                await _context.SaveChangesAsync();
                host.AARecords.Add(record);
                _jobService.Assign(host);
            }

            return record;
        }
    }
}