using System;
using System.Threading.Tasks;

using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Targets.Queries;
using pwnwrk.infra;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class ListModeHandler : IModeHandler
    {
        public string ModeName => "list";
        
        public async Task Handle(string[] args)
        {
            if (args.Length < 2 || args[1] != "--class")
            {
                Console.WriteLine("--class option is required");
                PrintHelpSection();
                return;
            }
            else if (args.Length < 3)
            {
                Console.WriteLine("No value provided for --class option");
                PrintHelpSection();
                return;
            }

            var @class = args[2].ToLower();
            var client = new PwnctlApiClient();

            if (@class == "hosts")
            {
                await client.Send(new ListHostsQuery());
            }
            if (@class == "endpoints")
            {
                await client.Send(new ListEndpointsQuery());
            }
            if (@class == "domains")
            {
                await client.Send(new ListDomainsQuery());
            }
            if (@class == "services")
            {
                await client.Send(new ListServicesQuery());
            }
            if (@class == "dnsrecords")
            {
                await client.Send(new ListDnsRecordsQuery());
            }
            if (@class == "netranges")
            {
                await client.Send(new ListNetRangesQuery());
            }
            if (@class == "emails")
            {
                await client.Send(new ListEmailsQuery());
            }
            if (@class == "targets")
            {
                var result = await client.Send(new ListTargetsQuery());
                Console.WriteLine(PwnContext.Serializer.Serialize(result));
            }
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