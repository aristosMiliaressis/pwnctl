using System;
using System.Threading.Tasks;

using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Targets.Queries;
using pwnwrk.infra;
using pwnctl.cli.Interfaces;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class ListModeHandler : ModeHandler
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

            object result = @class switch
            {
                "hosts" => await client.Send(new ListHostsQuery()),
                "endpoints" => await client.Send(new ListEndpointsQuery()),
                "domains" => await client.Send(new ListDomainsQuery()),
                "services" => await client.Send(new ListServicesQuery()),
                "netranges" => await client.Send(new ListNetRangesQuery()),
                "dnsrecords" => await client.Send(new ListDnsRecordsQuery()),
                "emails" => await client.Send(new ListEmailsQuery()),
                "targets" => await client.Send(new ListTargetsQuery()),
                 _ => throw new NotSupportedException("Not supported class " + @class)
            };
            
            Console.WriteLine(PwnContext.Serializer.Serialize(result));
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