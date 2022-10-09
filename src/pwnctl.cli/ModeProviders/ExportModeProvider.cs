using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using pwnctl.app.Repositories;
using pwnctl.core.BaseClasses;

namespace pwnctl.cli.ModeProviders
{
    public class ExportModeProvider : IModeProvider
    {
        public string ModeName => "export";
        
        public Task Run(string[] args)
        {
            void WriteToFile(string filename, IEnumerable<BaseAsset> assets)
            {
                assets.ToList().ForEach(a => File.AppendAllText(filename, a.ToJson() + "\n"));
            }

            if (args.Length < 2 || args[1] != "--path")
            {
                Console.WriteLine("--path option is required");
                PrintHelpSection();
                return Task.CompletedTask;
            }
            else if (args.Length < 3)
            {
                Console.WriteLine("No value provided for --path option");
                PrintHelpSection();
                return Task.CompletedTask;
            }

            var path = args[2];

            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create export directory {path}", ex);
            }

            AssetRepository repository = new();
            var hosts = repository.ListHosts().Select(a => (BaseAsset)a);
            WriteToFile(Path.Combine(path, "hosts.json"), hosts);
            var endpoints = repository.ListEndpoints().Select(a => (BaseAsset)a);
            WriteToFile(Path.Combine(path, "endpoints.json"), endpoints);
            var domains = repository.ListDomains().Select(a => (BaseAsset)a);
            WriteToFile(Path.Combine(path, "domains.json"), domains);
            var services = repository.ListServices().Select(a => (BaseAsset)a);
            WriteToFile(Path.Combine(path, "services.json"), services);
            var records = repository.ListDNSRecords().Select(a => (BaseAsset)a);
            WriteToFile(Path.Combine(path, "records.json"), records);
            var netRanges = repository.ListNetRanges().Select(a => (BaseAsset)a);
            WriteToFile(Path.Combine(path, "netRanges.json"), netRanges);
            var emails = repository.ListEmails().Select(a => (BaseAsset)a);
            WriteToFile(Path.Combine(path, "emails.json"), emails);

            return Task.CompletedTask;
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine($"\t\texports assets in jsonline format at the specified path.");
            Console.WriteLine($"\t\tArguments:");
            Console.WriteLine($"\t\t\t--path\tthe export path.");
        }
    }
}