using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using pwnctl.app.Repositories;
using pwnctl.core.BaseClasses;

namespace pwnctl.cli.ModeProviders
{
    public class ListModeProvider : IModeProvider
    {
        public string ModeName => "list";
        
        public Task Run(string[] args)
        {
            void WriteToConsole(IEnumerable<BaseAsset> assets)
            {
                assets.ToList().ForEach(a => Console.WriteLine(a.ToJson() + "\n"));
            }

            var @class = args[1];

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

        }
    }
}