using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using pwnctl.app;
using pwnctl.dto.Assets.Queries;
using pwnctl.cli.Interfaces;
using pwnctl.dto.Db.Commands;
using pwnctl.app.Assets.DTO;

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

            var domains = await client.Send(new ListDomainsQuery());
            WriteToFile(Path.Combine(path, "domains.json"), domains.Domains);

            var hosts = await client.Send(new ListHostsQuery());
            WriteToFile(Path.Combine(path, "hosts.json"), hosts.Hosts);

            var endpoints = await client.Send(new ListEndpointsQuery());
            WriteToFile(Path.Combine(path, "endpoints.json"), endpoints.Endpoints);

            var services = await client.Send(new ListServicesQuery());
            WriteToFile(Path.Combine(path, "services.json"), services.Services);

            var records = await client.Send(new ListDnsRecordsQuery());
            WriteToFile(Path.Combine(path, "records.json"), records.DNSRecords);

            var netRanges = await client.Send(new ListNetRangesQuery());
            WriteToFile(Path.Combine(path, "netRanges.json"), netRanges.NetRanges);

            var emails = await client.Send(new ListEmailsQuery());
            WriteToFile(Path.Combine(path, "emails.json"), emails.Emails);

            var results = await client.Send(new RunSqlQueryCommand // TODO: make this a strongly typed command
            { 
                Query = "SELECT \"TaskEntries\".\"Id\",\"ExitCode\",\"State\",\"QueuedAt\",\"StartedAt\",\"FinishedAt\",\"SubjectClass_Class\",\"ShortName\" FROM \"TaskEntries\" JOIN \"TaskDefinitions\" ON \"TaskEntries\".\"DefinitionId\" = \"TaskDefinitions\".\"Id\";"
            });

            foreach (var row in results)
            {
                File.AppendAllText(Path.Combine(path, "tasks.json"), row + "\n");
            }
        }
        
        private void WriteToFile(string filename, IEnumerable<AssetDTO> assets)
        {
            foreach (var asset in assets)
            {
                File.AppendAllText(filename, PwnInfraContext.Serializer.Serialize(asset) + "\n");
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