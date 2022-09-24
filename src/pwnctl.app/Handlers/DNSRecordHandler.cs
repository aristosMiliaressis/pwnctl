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
                await _jobService.AssignAsync(domain);
            }

            if (host != null && !host.InScope)
            {
                // this prevents some db errors
                _context.ChangeTracker.TrackGraph(host, e =>
                {
                    if (e.Entry.Entity is Domain)
                        e.Entry.State = EntityState.Detached;
                });
            
                host.InScope = true;
                await _context.SaveChangesAsync();
                host.AARecords.Add(record);
                await _jobService.AssignAsync(host);
            }

            return record;
        }
    }
}