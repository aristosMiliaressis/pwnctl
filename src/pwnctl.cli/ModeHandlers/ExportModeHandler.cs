using CommandLine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using pwnctl.app;
using pwnctl.dto.Assets.Queries;
using pwnctl.cli.Interfaces;
using pwnctl.app.Assets.DTO;
using pwnctl.dto.Tasks.Queries;
using pwnctl.dto.Assets.Models;
using pwnctl.dto.Mediator;
using pwnctl.dto.Tasks.Models;

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

                var netRanges = await Program.Sender.Send<MediatedResponse<NetworkRangeListViewModel>>(new ListNetworkRangesQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "network_ranges.json"), netRanges.Result.Rows);

                var hosts = await Program.Sender.Send<MediatedResponse<NetworkHostListViewModel>>(new ListNetworkHostsQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "network_hosts.json"), hosts.Result.Rows);

                var services = await Program.Sender.Send<MediatedResponse<NetworkSocketListViewModel>>(new ListNetworkSocketsQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "network_services.json"), services.Result.Rows);

                var domains = await Program.Sender.Send<MediatedResponse<DomainNameListViewModel>>(new ListDomainNamesQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "domain_names.json"), domains.Result.Rows);

                var records = await Program.Sender.Send<MediatedResponse<DomainNameRecordListViewModel>>(new ListDomainNameRecordsQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "domain_name_records.json"), records.Result.Rows);

                var vhosts = await Program.Sender.Send<MediatedResponse<VirtualHostListViewModel>>(new ListVirtualHostsQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "virtual_hosts.json"), vhosts.Result.Rows);

                var emails = await Program.Sender.Send<MediatedResponse<EmailListViewModel>>(new ListEmailsQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "emails.json"), emails.Result.Rows);

                var parameters = await Program.Sender.Send<MediatedResponse<HttpParameterListViewModel>>(new ListHttpParametersQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "http_parameters.json"), parameters.Result.Rows);

                var tasks = await Program.Sender.Send<MediatedResponse<TaskRecordListViewModel>>(new ListTaskRecordsQuery());
                foreach (var task in tasks.Result.Rows)
                {
                    File.AppendAllText(Path.Combine(opt.ExportPath, "task_records.json"), PwnInfraContext.Serializer.Serialize(task) + "\n");
                }

                var endpoints = await Program.Sender.Send<MediatedResponse<HttpEndpointListViewModel>>(new ListHttpEndpointsQuery());
                WriteToFile(Path.Combine(opt.ExportPath, "http_endpoints.json"), endpoints.Result.Rows);
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