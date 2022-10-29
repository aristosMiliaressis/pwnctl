using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using pwnwrk.infra.Persistence;

namespace pwnctl.cli.ModeHandlers
{
    public class SummaryModeHandler : IModeHandler
    {
        public string ModeName => "summary";
        
        public async Task Handle(string[] args)
        {
            PwnctlDbContext context = new();
            int netRangeCount = await context.NetRanges.CountAsync();
            int hostCount = await context.Hosts.CountAsync();
            int domainCount = await context.Domains.CountAsync();
            int recordCount = await context.DNSRecords.CountAsync();
            int serviceCount = await context.Services.CountAsync();
            int endpointCount = await context.Endpoints.CountAsync();
            int paramCount = await context.Parameters.CountAsync();
            int emailCount = await context.Emails.CountAsync();
            int tagCount = await context.Tags.CountAsync();
            int inScopeRangesCount = await context.NetRanges.Where(a => a.InScope).CountAsync();
            int insCopeHostCount = await context.Hosts.CountAsync();
            int inScopeDomainCount = await context.Domains.Where(a => a.InScope).CountAsync();
            int inScopeRecordCount = await context.DNSRecords.Where(a => a.InScope).CountAsync();
            int inScopeServiceCount = await context.Services.Where(a => a.InScope).CountAsync();
            int inScopeEndpointCount = await context.Endpoints.Where(a => a.InScope).CountAsync();
            int inScopeParamCount = await context.Parameters.Where(a => a.InScope).CountAsync();
            int inScopeEmailCount = await context.Emails.Where(a => a.InScope).CountAsync();
            var firstTask = await context.Tasks.OrderBy(t => t.QueuedAt).FirstOrDefaultAsync();
            var lastTask = await context.Tasks.OrderBy(t => t.QueuedAt).FirstOrDefaultAsync();

            Console.WriteLine($"NetRanges: {netRangeCount}, InScope: {inScopeRangesCount}");
            Console.WriteLine($"Hosts: {hostCount}, InScope: {insCopeHostCount}");
            Console.WriteLine($"Domains: {domainCount}, InScope: {inScopeDomainCount}");
            Console.WriteLine($"DNSRecords: {recordCount}, InScope: {inScopeRecordCount}");
            Console.WriteLine($"Services: {serviceCount}, InScope: {inScopeServiceCount}");
            Console.WriteLine($"Endpoints: {endpointCount}, InScope: {inScopeEndpointCount}");
            Console.WriteLine($"Parameters: {paramCount}, InScope: {inScopeParamCount}");
            Console.WriteLine($"Emais: {emailCount}, InScope: {inScopeEmailCount}");
            Console.WriteLine($"Tags: {tagCount}");
            if (firstTask != null)
            {
                Console.WriteLine();
                Console.WriteLine("First Queued Task: " + firstTask.QueuedAt);
                Console.WriteLine("Last Queued Task: " + lastTask.QueuedAt);
            }
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine($"\t\tprints a summary of the found assets and queued tasks");
        }
    }
}