﻿using pwnctl.cli.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using pwnctl.infra.DependencyInjection;
using MediatR;
using pwnctl.cli;
using Microsoft.Extensions.DependencyInjection;
using pwnctl.dto;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.infra.Queueing;
using pwnctl.api;
using pwnctl.api.Mediator.Pipelines;
using FluentValidation;
using pwnctl.infra.Configuration;
using pwnctl.app;

internal sealed class Program
{
    static Dictionary<string, ModeHandler> _modeProviders =
            AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => !t.IsInterface && typeof(ModeHandler).IsAssignableFrom(t))
                    .Select(t => (ModeHandler)Activator.CreateInstance(t))
                    .ToDictionary(p => p.ModeName, p => p);

    public static ISender Sender = CreateSender();

    static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("No mode provided");
            PrintHelpPage();
            return;
        }

        var mode = args[0];

        if (!_modeProviders.ContainsKey(mode))
        {
            Console.WriteLine("Invalid Mode " + mode);
            PrintHelpPage();
            return;
        }

        var provider = _modeProviders[mode];

        await provider.Handle(args);
    }

    static ISender CreateSender()
    {
        PwnInfraContextInitializer.Setup();

        if (PwnInfraContext.Config.Api.Bypass)
        {
            var services = new ServiceCollection();
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(PwnctlDtoAssemblyMarker).GetTypeInfo().Assembly);
                cfg.RegisterServicesFromAssemblies(typeof(PwnctlApiAssemblyMarker).GetTypeInfo().Assembly);
            });
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuditLoggingPipeline<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));
            services.AddValidatorsFromAssemblyContaining<PwnctlDtoAssemblyMarker>();
            services.AddTransient<TaskQueueService, SQSTaskQueueService>();
            return new Mediator(services.BuildServiceProvider());
        }
        else
        {
            return PwnctlApiClient.Default;
        }
    }

    static void PrintHelpPage()
    {
        Console.WriteLine();
        Console.WriteLine($"USAGE: {Assembly.GetExecutingAssembly().GetName().Name} <mode> [arguments]");
        Console.WriteLine();
        Console.WriteLine("MODES:");
        foreach (var provider in _modeProviders.Values)
        {
            provider.PrintHelpSection();
            Console.WriteLine();
        }
    }
}
