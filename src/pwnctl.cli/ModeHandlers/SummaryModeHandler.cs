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
            var model = await PwnctlApiClient.Default.Send(new SummaryQuery());

            Console.WriteLine($"NetworkRanges: {model.NetworkRangeCount}, InScope: {model.InScopeRangesCount}");
            Console.WriteLine($"NetworkHosts: {model.HostCount}, InScope: {model.InScopeHostCount}");
            Console.WriteLine($"NetworkSockets: {model.SocketCount}, InScope: {model.InScopeServiceCount}");
            Console.WriteLine($"DomainNames: {model.DomainCount}, InScope: {model.InScopeDomainCount}");
            Console.WriteLine($"DomainNameRecords: {model.RecordCount}, InScope: {model.InScopeRecordCount}");
            Console.WriteLine($"HttpEndpoints: {model.HttpEndpointCount}, InScope: {model.InScopeEndpointCount}");
            Console.WriteLine($"HttpParameters: {model.HttpParamCount}, InScope: {model.InScopeParamCount}");
            Console.WriteLine($"Emais: {model.EmailCount}, InScope: {model.InScopeEmailCount}");
            Console.WriteLine($"Tags: {model.TagCount}");
            Console.WriteLine();
            Console.WriteLine($"QUEUED: {model.QueuedTaskCount}, RUNNING: {model.RunningTaskCount}, FINISHED: {model.FinishedTaskCount}, FAILED: {model.FailedTaskCount}");
            Console.WriteLine();
            foreach(var def in model.TaskDetails.OrderBy(t => t.Count))
            {
                Console.WriteLine($"{def.ShortName.PadLeft(24)}: Queued {def.Count.ToString().PadLeft(4)} times, ran for {def.Duration.ToString("dd\\.hh\\:mm\\:ss")} and found {def.Findings.ToString().PadLeft(4)} unique assets.");
            }

            if (model.FirstTask is not null)
            {
                Console.WriteLine();
                Console.WriteLine("First Queued Task: " + model.FirstTask);
                Console.WriteLine("Last Queued Task: " + model.LastTask);
                Console.WriteLine("Last Finished Task: " + model.LastFinishedTask);

                // var productiveTime = TimeSpan.FromSeconds(model.TaskDetails.Sum(t => t.Duration.TotalSeconds));
                // Console.WriteLine("Total Productive time: " + productiveTime.ToString("dd\\.hh\\:mm\\:ss"));
            }
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}\tprints a summary of the found assets and queued tasks");
        }
    }
}
