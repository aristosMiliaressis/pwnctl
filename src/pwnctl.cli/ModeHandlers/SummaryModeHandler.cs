using System;
using System.Linq;
using System.Threading.Tasks;

using pwnwrk.infra.Persistence;

namespace pwnctl.cli.ModeHandlers
{
    public class SummaryModeHandler : IModeHandler
    {
        public string ModeName => "summary";
        
        public Task Handle(string[] args)
        {
            PwnctlDbContext context = new();
            int netRangeCount = context.NetRanges.Count();
            int hostCount = context.Hosts.Count();
            int domainCount = context.Domains.Count();
            int recordCount = context.DNSRecords.Count();
            int serviceCount = context.Services.Count();
            int endpointCount = context.Endpoints.Count();
            int paramCount = context.Parameters.Count();
            int emailCount = context.Emails.Count();
            int tagCount = context.Tags.Count();
            int inScopeRangesCount = context.NetRanges.Where(a => a.InScope).Count();
            int insCopeHostCount = context.Hosts.Count();
            int inScopeDomainCount = context.Domains.Where(a => a.InScope).Count();
            int inScopeRecordCount = context.DNSRecords.Where(a => a.InScope).Count();
            int inScopeServiceCount = context.Services.Where(a => a.InScope).Count();
            int inScopeEndpointCount = context.Endpoints.Where(a => a.InScope).Count();
            int inScopeParamCount = context.Parameters.Where(a => a.InScope).Count();
            int inScopeEmailCount = context.Emails.Where(a => a.InScope).Count();
            var firstTask = context.Tasks.OrderBy(t => t.QueuedAt).FirstOrDefault();
            var lastTask = context.Tasks.OrderBy(t => t.QueuedAt).LastOrDefault();

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

            return Task.CompletedTask;
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine($"\t\tprints a summary of the found assets and queued tasks");
        }
    }
}