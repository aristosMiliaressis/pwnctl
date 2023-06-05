using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using pwnctl.app;
using pwnctl.dto.Assets.Queries;
using pwnctl.cli.Interfaces;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Tasks.Queries;
using CommandLine;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class ExportModeHandler : ModeHandler
    {
        public string ModeName => "export";

        [Option('p', "path", Required = true, HelpText = "Path to the export file.")]
        public string ExportPath { get; set; }

        public async Task Handle(string[] args)
        {
            await Parser.Default.ParseArguments<ExportModeHandler>(args).WithParsedAsync(async opt =>
            {
                try
                {
                    Directory.CreateDirectory(opt.ExportPath);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unable to create export directory {opt.ExportPath}", ex);
                }

                var domains = await PwnctlApiClient.Default.Send(new ListDomainsQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "domains.json"), domains.Rows);

                var hosts = await PwnctlApiClient.Default.Send(new ListHostsQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "hosts.json"), hosts.Rows);

                var endpoints = await PwnctlApiClient.Default.Send(new ListEndpointsQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "endpoints.json"), endpoints.Rows);

                var services = await PwnctlApiClient.Default.Send(new ListServicesQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "services.json"), services.Rows);

                var records = await PwnctlApiClient.Default.Send(new ListDnsRecordsQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "records.json"), records.Rows);

                var netRanges = await PwnctlApiClient.Default.Send(new ListNetRangesQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "netRanges.json"), netRanges.Rows);

                var emails = await PwnctlApiClient.Default.Send(new ListEmailsQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "emails.json"), emails.Rows);

                var parameters = await PwnctlApiClient.Default.Send(new ListParametersQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "parameters.json"), parameters.Rows);

                var tasks = await PwnctlApiClient.Default.Send(new ListTaskEntriesQuery());
                foreach (var task in tasks.Rows)
                {
                    File.AppendAllText(Path.Combine(opt.ExportPath, "tasks.json"), PwnInfraContext.Serializer.Serialize(task) + "\n");
                }
            });
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
            Console.WriteLine($"\t{ModeName}\texports assets in jsonline format at the specified path.");
            Console.WriteLine($"\t\t--path\tthe export path.");
        }
    }
}