using System;
using System.Threading.Tasks;

using pwnctl.cli.Interfaces;
using pwnctl.app;
using pwnctl.dto.Operations.Commands;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class OnboardTargetModeHandler : ModeHandler
    {
        public string ModeName => "onboard";

        public async Task Handle(string[] args)
        {
            string line, json = string.Empty;

            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                json += line + "\n";
            }

            var command = PwnInfraContext.Serializer.Deserialize<CreateOperationCommand>(json);

            var client = new PwnctlApiClient();
            await client.Send(command);
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine("\t\tOnboard target program (reads target json from stdin)");
        }
    }
}