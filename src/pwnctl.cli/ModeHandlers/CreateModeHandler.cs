using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using pwnctl.app;
using pwnctl.cli.Interfaces;
using pwnctl.dto.Mediator;
using pwnctl.dto.Operations.Commands;
using pwnctl.dto.Scope.Commands;
using pwnctl.dto.Tasks.Commands;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace pwnctl.cli.ModeHandlers
{
    public class CreateModeHandler : ModeHandler
    {
        public string ModeName => "create";

        [Option('r', "resource", Required = true, HelpText = "The resource type to be created.")]
        public string Resource { get; set; }

        private static Dictionary<string, Type> ResourceMap = new()
        {
            { "ops", typeof(CreateOperationCommand) },
            { "scope", typeof(CreateScopeAggregateCommand) },
            { "task-profiles", typeof(CreateTaskProfileCommand) },
            { "task-definitions", typeof(CreateTaskDefinitionCommand) }
        };

        public async Task Handle(string[] args)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            await Parser.Default.ParseArguments<CreateModeHandler>(args).WithParsedAsync(async opt =>
            {
                string line = string.Empty;
                string yaml = string.Empty;
                do
                {
                    yaml += line + "\n";
                    line = Console.ReadLine();
                }
                while (line != null);

                var request = (MediatedRequest)deserializer.Deserialize(yaml, ResourceMap[opt.Resource]);

                await Program.Sender.Send(request);
            });
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}\tCreate a resource");
            Console.WriteLine($"\t\t-r, --resource\t");
            foreach (var resource in ResourceMap.Keys)
            {
                Console.WriteLine($"\t\t\t{resource}");
            }
        }
    }
}