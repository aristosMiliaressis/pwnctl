using System;
using System.Threading.Tasks;

using pwnctl.dto.Db.Queries;
using pwnctl.cli.Interfaces;
using System.Linq;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class SummaryModeHandler : ModeHandler
    {
        public string ModeName => "summary";
        
        public async Task Handle(string[] args)
        {
            var client = new PwnctlApiClient();
            var model = await client.Send(new SummaryQuery());

            Console.WriteLine($"NetRanges: {model.NetRangeCount}, InScope: {model.InScopeRangesCount}");
            Console.WriteLine($"Hosts: {model.HostCount}, InScope: {model.InsCopeHostCount}");
            Console.WriteLine($"Domains: {model.DomainCount}, InScope: {model.InScopeDomainCount}");
            Console.WriteLine($"DNSRecords: {model.RecordCount}, InScope: {model.InScopeRecordCount}");
            Console.WriteLine($"Services: {model.ServiceCount}, InScope: {model.InScopeServiceCount}");
            Console.WriteLine($"Endpoints: {model.EndpointCount}, InScope: {model.InScopeEndpointCount}");
            Console.WriteLine($"Parameters: {model.ParamCount}, InScope: {model.InScopeParamCount}");
            Console.WriteLine($"Emais: {model.EmailCount}, InScope: {model.InScopeEmailCount}");
            Console.WriteLine($"Tags: {model.TagCount}");
            Console.WriteLine();
            Console.WriteLine($"PENDING: {model.PendingTaskCount}, QUEUED: {model.QueuedTaskCount}, RUNNING: {model.RunningTaskCount}, FINISHED: {model.FinishedTaskCount}");
            if (model.FirstTask != null)
            {
                Console.WriteLine();
                Console.WriteLine("First Queued Task: " + model.FirstTask);
                Console.WriteLine("Last Queued Task: " + model.LastTask);
            }

            Console.WriteLine();
            foreach(var def in model.TaskDetails.OrderBy(t => t.Duration))
            {
                Console.WriteLine($"{def.ShortName.PadLeft(25)}: {def.Count.ToString().PadLeft(5)} - {def.Duration}");
            }
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine($"\t\tprints a summary of the found assets and queued tasks");
        }
    }
}
