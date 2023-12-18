using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.FileSystemGlobbing;
using pwnctl.app;
using pwnctl.cli.Interfaces;
using pwnctl.infra.Configuration.Validation;
using pwnctl.infra.Configuration.Validation.Exceptions;

namespace pwnctl.cli.ModeHandlers
{
    public class ValidateModeHandler : ModeHandler
    {
        public string ModeName => "validate";

        [Option('p', "path", Required = true, HelpText = "Path of seed files.")]
        public string Path { get; set; }

        public Task Handle(string[] args)
        {
            // Parser.Default.ParseArguments<ValidateModeHandler>(args).WithParsed(opt => 
            // {
            //     Matcher matcher = new();
            //     matcher.AddInclude("*.td.yml");

            //     foreach (string taskFile in matcher.GetResultsInFullPath(opt.Path))
            //     {
            //         if (!File.Exists(taskFile))
            //         {
            //             throw new ConfigValidationException(taskFile, "File not found");
            //         }

            //         var passed = ConfigValidator.TryValidateTaskDefinitions(taskFile, out string errorMessage);
            //         if (!passed)
            //         {
            //             throw new ConfigValidationException(taskFile, errorMessage);
            //         }
            //     }

            //     matcher = new();
            //     matcher.AddInclude("*.nr.yml");

            //     foreach (string notificationFile in matcher.GetResultsInFullPath(opt.Path))
            //     {
            //         if (!File.Exists(notificationFile))
            //         {
            //             throw new ConfigValidationException(notificationFile, "File not found");
            //         }

            //         var passed = ConfigValidator.TryValidateNotificationRules(notificationFile, out string errorMessage);
            //         if (!passed)
            //         {
            //             throw new ConfigValidationException(notificationFile, errorMessage);
            //         }
            //     }
            // });

            return Task.CompletedTask;
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}\tValidate seed files");
            Console.WriteLine($"\t\t-p, --path\t");
        }
    }
}