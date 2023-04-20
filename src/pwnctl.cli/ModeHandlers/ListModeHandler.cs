using System;
using System.Threading.Tasks;

using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Operations.Queries;
using pwnctl.app;
using pwnctl.cli.Interfaces;
using pwnctl.dto.Scope.Queries;
using pwnctl.dto.Mediator;
using pwnctl.dto.Tasks.Queries;
using System.Collections.Generic;
using CommandLine;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class ListModeHandler : ModeHandler
    {
        public string ModeName => "list";

        [Option('r', "resource", Required = true, HelpText = "The resource type to be created.")]
        public string Resource { get; set; }

        private static Dictionary<string, Type> ResourceMap = new()
        {
            { "hosts", typeof(ListHostsQuery) },
            { "endpoints", typeof(ListEndpointsQuery) },
            { "domains", typeof(ListDomainsQuery) },
            { "services", typeof(ListServicesQuery) },
            { "netranges", typeof(ListNetRangesQuery) },
            { "dnsrecords", typeof(ListDnsRecordsQuery) },
            { "emails", typeof(ListEmailsQuery) },
            { "ops", typeof(ListOperationsQuery) },
            { "scope", typeof(ListScopeAggregatesQuery) },
            { "task-profiles", typeof(ListTaskProfilesQuery) },
            { "task-definitions", typeof(ListTaskDefinitionsQuery) }
        };

        public async Task Handle(string[] args)
        {
            await Parser.Default.ParseArguments<ListModeHandler>(args).WithParsedAsync(async opt =>
            {
                var request = (MediatedRequest<object>)Activator.CreateInstance(ResourceMap[opt.Resource]);

                object result = await PwnctlApiClient.Default.Send(request);

                Console.WriteLine(PwnInfraContext.Serializer.Serialize(result));
            });
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine($"\t\tlists asset of the specified sealed class in jsonline format.");
            Console.WriteLine($"\t\tArguments:");
            Console.WriteLine($"\t\t\t-r, --resource\t");
            foreach (var resource in ResourceMap.Keys)
            {
                Console.WriteLine($"\t\t\t\t{resource}");
            }
        }
    }
}