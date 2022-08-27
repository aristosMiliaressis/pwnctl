using Microsoft.EntityFrameworkCore;
using pwnctl.core.Entities.Assets;

namespace pwnctl.app.Handlers
{
    public class HostHandler : BaseAssetHandler<Host>
    {
        protected override async Task<Host> HandleAsync(Host host)
        {
            host.AARecords = await _context.DNSRecords.Where(r => (r.Type == DNSRecord.RecordType.A || r.Type == DNSRecord.RecordType.AAAA)
                                                        && r.Value == host.IP).ToListAsync();

            return host;
        }
    }
}