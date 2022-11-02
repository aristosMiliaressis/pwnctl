using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using pwnwrk.infra.Repositories;
using pwnwrk.domain.Assets.BaseClasses;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class ListModeHandler : IModeHandler
    {
        public string ModeName => "list";
        
        public Task Handle(string[] args)
        {
            void WriteToConsole(IEnumerable<BaseAsset> assets)
            {
                assets.ToList().ForEach(a => Console.WriteLine(a.ToJson() + "\n"));
            }

            if (args.Length < 2 || args[1] != "--class")
            {
                Console.WriteLine("--class option is required");
                PrintHelpSection();
                return Task.CompletedTask;
            }
            else if (args.Length < 3)
            {
                Console.WriteLine("No value provided for --class option");
                PrintHelpSection();
                return Task.CompletedTask;
            }

            var @class = args[2];

            AssetRepository repository = new();
            if (@class.ToLower() == "hosts")
            {
                var assets = repository.ListHosts().Select(a => (BaseAsset)a);
                WriteToConsole(assets);
            }
            if (@class.ToLower() == "endpoints")
            {
                var assets = repository.ListEndpoints().Select(a => (BaseAsset)a);
                WriteToConsole(assets);
            }
            if (@class.ToLower() == "domains")
            {
                var assets = repository.ListDomains().Select(a => (BaseAsset)a);
                WriteToConsole(assets);
            }
            if (@class.ToLower() == "services")
            {
                var assets = repository.ListServices().Select(a => (BaseAsset)a);
                WriteToConsole(assets);
            }
            if (@class.ToLower() == "dnsrecords")
            {
                var assets = repository.ListDNSRecords().Select(a => (BaseAsset)a);
                WriteToConsole(assets);
            }
            if (@class.ToLower() == "netranges")
            {
                var assets = repository.ListNetRanges().Select(a => (BaseAsset)a);
                WriteToConsole(assets);
            }
            if (@class.ToLower() == "emails")
            {
                var assets = repository.ListEmails().Select(a => (BaseAsset)a);
                WriteToConsole(assets);
            }

            return Task.CompletedTask;
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine($"\t\tlists asset of the specified sealed class in jsonline format.");
            Console.WriteLine($"\t\tArguments:");
            Console.WriteLine($"\t\t\t--class\tthe asset sealed class (hosts/endpoints/domains/services/dnsrecords/netranges/emails).");
        }
    }
}