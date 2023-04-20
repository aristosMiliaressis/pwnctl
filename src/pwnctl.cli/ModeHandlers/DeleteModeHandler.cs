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
    public class DeleteModeHandler : ModeHandler
    {
        public string ModeName => "delete";

        [Option('r', "resource", Required = true, HelpText = "The resource type to be deleted.")]
        public string Resource { get; set; }

        [Option('n', "name", Required = true, HelpText = "The resource ShortName to be deleted.")]
        public string ShortName { get; set; }


        private static Dictionary<string, Type> ResourceMap = new()
        {
            { "ops", typeof(DeleteOperationCommand) },
            { "scope", typeof(DeleteScopeAggregateCommand) },
            { "task-profiles", typeof(DeleteTaskProfileCommand) },
            { "task-definitions", typeof(DeleteTaskDefinitionCommand) }
        };

        public async Task Handle(string[] args)
        {
            await Parser.Default.ParseArguments<DeleteModeHandler>(args).WithParsedAsync(async opt =>
            {
                var request = (MediatedRequest)PwnInfraContext.Serializer.Deserialize($"{{\"ShortName\":\"{opt.ShortName}\"}}", ResourceMap[opt.Resource]);

                await PwnctlApiClient.Default.Send(request);
            });
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}\tDelete a resource");
            Console.WriteLine($"\t\t-r, --resource\t");
            foreach (var resource in ResourceMap.Keys)
            {
                Console.WriteLine($"\t\t\t{resource}");
            }
        }
    }
}