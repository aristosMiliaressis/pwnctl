using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Common.Interfaces;
using pwnctl.dto.Assets.Queries;
using pwnctl.cli.Interfaces;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class ExportModeHandler : ModeHandler
    {
        public string ModeName => "export";
        
        public async Task Handle(string[] args)
        {
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
            WriteToFile(Path.Combine(path, "hosts.json"), hosts.Hosts);

            var endpoints = await client.Send(new ListEndpointsQuery());
            WriteToFile(Path.Combine(path, "endpoints.json"), endpoints.Endpoints);

            var domains = await client.Send(new ListDomainsQuery());
            WriteToFile(Path.Combine(path, "domains.json"), domains.Domains);

            var services = await client.Send(new ListServicesQuery());
            WriteToFile(Path.Combine(path, "services.json"), services.Services);

            var records = await client.Send(new ListDnsRecordsQuery());
            WriteToFile(Path.Combine(path, "records.json"), records.DNSRecords);

            var netRanges = await client.Send(new ListNetRangesQuery());
            WriteToFile(Path.Combine(path, "netRanges.json"), netRanges.NetRanges);

            var emails = await client.Send(new ListEmailsQuery());
            WriteToFile(Path.Combine(path, "emails.json"), emails.Emails);
        }
        
        private void WriteToFile(string filename, IEnumerable<Asset> assets)
        {
            foreach (var asset in assets)
            {
                var dto = new AssetRecord(asset).ToDTO();
                File.AppendAllText(filename, Serializer.Instance.Serialize(dto) + "\n");
            }
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