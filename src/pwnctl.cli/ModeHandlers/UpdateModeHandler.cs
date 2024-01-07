using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;
using pwnctl.app;
using pwnctl.cli.Interfaces;
using pwnctl.dto.Mediator;
using pwnctl.dto.Operations.Commands;
using pwnctl.dto.Scope.Commands;
using pwnctl.dto.Tasks.Commands;

namespace pwnctl.cli.ModeHandlers
{
    public class UpdateModeHandler : ModeHandler
    {
        public string ModeName => "update";

        [Option('r', "resource", Required = true, HelpText = "The resource type to be deleted.")]
        public string Resource { get; set; }

        [Option('n', "name", Required = true, HelpText = "The resource Name to be deleted.")]
        public string Name { get; set; }

        private static Dictionary<string, Type> ResourceMap = new()
        {
            { "scope", typeof(UpdateScopeAggregateCommand) },
            { "task-profiles", typeof(UpdateTaskProfileCommand) },
            { "task-definitions", typeof(UpdateTaskDefinitionCommand) }
        };

        public async Task Handle(string[] args)
        {
            string line, json = string.Empty;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                json += line + "\n";
            }

            await Parser.Default.ParseArguments<UpdateModeHandler>(args).WithParsedAsync(async opt =>
            {
                var request = (MediatedRequest)PwnInfraContext.Serializer.Deserialize(json, ResourceMap[opt.Resource]);

                await PwnctlApiClient.Default.Send(request);
            });
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}\tUpdate a resource");
            Console.WriteLine($"\t\t-r, --resource\t");
            foreach (var resource in ResourceMap.Keys)
            {
                Console.WriteLine($"\t\t\t{resource}");
            }
        }
    }
}