using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using pwnctl.dto.Targets.Commands;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class OnboardingTargetModeHandler : IModeHandler
    {
        public string ModeName => "onboard";

        public async Task Handle(string[] args)
        {
            string json = string.Empty;
            while (!string.IsNullOrEmpty(json += Console.ReadLine()));

            var command = JsonSerializer.Deserialize<CreateTargetCommand>(json);

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