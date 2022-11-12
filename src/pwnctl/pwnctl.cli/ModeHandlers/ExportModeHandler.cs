using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using pwnctl.dto.Assets.Queries;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.infra;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class ExportModeHandler : IModeHandler
    {
        public string ModeName => "export";
        
        public async Task Handle(string[] args)
        {
            void WriteToFile(string filename, IEnumerable<Asset> assets)
            {
                assets.ToList().ForEach(a => File.AppendAllText(filename, PwnContext.Serializer.Serialize(a.ToDTO()) + "\n"));
            }

            if (args.Length < 2 || args[1] != "--path")
            {
                Console.WriteLine("--path option is required");
                PrintHelpSection();
                return;
            }
            else if (args.Length < 3)
            {
                Console.WriteLine("No value provided for --path option");
                PrintHelpSection();
                return;
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

            var client = new PwnctlApiClient();

            var hosts = await client.Send(new ListHostsQuery());
            WriteToFile(Path.Combine(path, "hosts.json"), hosts.Hosts.Select(a => (Asset)a));
            var endpoints = await client.Send(new ListEndpointsQuery());
            WriteToFile(Path.Combine(path, "endpoints.json"), endpoints.Endpoints.Select(a => (Asset)a));
            var domains = await client.Send(new ListDomainsQuery());
            WriteToFile(Path.Combine(path, "domains.json"), domains.Domains.Select(a => (Asset)a));
            var services = await client.Send(new ListServicesQuery());
            WriteToFile(Path.Combine(path, "services.json"), services.Services.Select(a => (Asset)a));
            var records = await client.Send(new ListDnsRecordsQuery());
            WriteToFile(Path.Combine(path, "records.json"), records.DNSRecords.Select(a => (Asset)a));
            var netRanges = await client.Send(new ListNetRangesQuery());
            WriteToFile(Path.Combine(path, "netRanges.json"), netRanges.NetRanges.Select(a => (Asset)a));
            var emails = await client.Send(new ListEmailsQuery());
            WriteToFile(Path.Combine(path, "emails.json"), emails.Emails.Select(a => (Asset)a));
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