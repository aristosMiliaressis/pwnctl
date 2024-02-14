using System;
using System.Threading.Tasks;

using pwnctl.dto.Operations.Queries;
using pwnctl.cli.Interfaces;
using System.Linq;
using CommandLine;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class SummaryModeHandler : ModeHandler
    {
        public string ModeName => "summary";

        [Option('n', "name", Required = true, HelpText = "The operations name.")]
        public string Name { get; set; }

        public async Task Handle(string[] args)
        {
            await Parser.Default.ParseArguments<SummaryModeHandler>(args).WithParsedAsync(async opt =>
            {
                var model = await PwnctlApiClient.Default.Send(new OperationSummaryQuery { Name = opt.Name });

                Console.WriteLine($"Name: {model.Name}\tType: {model.Type}\tState: {model.State}");
                Console.WriteLine($"Initiated At: {model.InitializedAt}\tFinished At: {model.FinishedAt}\tCurrent Phase: {model.CurrentPhase}");
                Console.WriteLine();

                if (model.FirstTask is not null)
                {
                    Console.WriteLine("First Queued Task: " + model.FirstTask);
                    Console.WriteLine("Last Queued Task: " + model.LastTask);
                    Console.WriteLine("Last Finished Task: " + model.LastFinishedTask);
                    Console.WriteLine();
                    var shortLivedTaskExecTime = TimeSpan.FromSeconds(model.TaskDetails.Where(t => t.ShortLived).Select(t => t.Duration.TotalSeconds).Sum());
                    var longLivedTaskExecTime = TimeSpan.FromSeconds(model.TaskDetails.Where(t => !t.ShortLived).Select(t => t.Duration.TotalSeconds).Sum());
                    var subExecTime = shortLivedTaskExecTime + longLivedTaskExecTime;
                    Console.WriteLine($"Total Task Execution Time: {subExecTime:dd\\.hh\\:mm\\:ss} of which {shortLivedTaskExecTime:dd\\.hh\\:mm\\:ss} on spot instances and {longLivedTaskExecTime:dd\\.hh\\:mm\\:ss} on EC2 instances.");
                    Console.WriteLine();
                }

                Console.WriteLine($"NetworkRanges: {model.InScopeRangesCount}");
                Console.WriteLine($"NetworkHosts: {model.InScopeHostCount}");
                Console.WriteLine($"NetworkSockets: {model.InScopeServiceCount}");
                Console.WriteLine($"DomainNames: {model.InScopeDomainCount}");
                Console.WriteLine($"DomainNameRecords: {model.InScopeRecordCount}");
                Console.WriteLine($"HttpEndpoints: {model.InScopeEndpointCount}");
                Console.WriteLine($"HttpParameters: {model.InScopeParamCount}");
                Console.WriteLine($"VirtualHosts: {model.InScopeVirtualHostCount}");
                Console.WriteLine($"Emais: {model.InScopeEmailCount}");
                Console.WriteLine($"Tags: {model.TagCount}");
                Console.WriteLine();
                Console.WriteLine($"QUEUED: {model.QueuedTaskCount}, RUNNING: {model.RunningTaskCount}, FINISHED: {model.FinishedTaskCount}, CANCELED: {model.CanceledTaskCount}, TIMED_OUT: {model.TimedOutTaskCount}, FAILED: {model.FailedTaskCount}");
                Console.WriteLine();
                foreach (var def in model.TaskDetails.Where(t => t.ShortLived).OrderBy(t => t.Duration))
                    Console.WriteLine($"S {def.Name,20}: Queued {def.Count,4} times, ran {def.RunCount,4} times, for {def.Duration:dd\\.hh\\:mm\\:ss} and found {def.Findings,6} unique assets.");
                foreach (var def in model.TaskDetails.Where(t => !t.ShortLived).OrderBy(t => t.Duration))
                    Console.WriteLine($"L {def.Name,20}: Queued {def.Count,4} times, ran {def.RunCount,4} times, for {def.Duration:dd\\.hh\\:mm\\:ss} and found {def.Findings,6} unique assets.");
           });
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}\tprints a summary of the found assets and queued tasks");
        }
    }
}
