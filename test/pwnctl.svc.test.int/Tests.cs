namespace pwnctl.svc.test.integration;

using pwnctl.domain.BaseClasses;
using pwnctl.domain.Entities;
using pwnctl.domain.ValueObjects;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Scope.Enums;
using pwnctl.app.Assets.Aggregates;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DotNet.Testcontainers.Builders;

public sealed class Tests
{
    public Tests()
    {
        Directory.CreateDirectory("/__w/pwnctl/pwnctl/test/pwnctl.svc.test.int/deployment/seed");

        foreach (var file in Directory.GetFiles("../../../../../src/core/pwnctl.infra/Persistence/seed"))
            File.Copy(file, Path.Combine("/__w/pwnctl/pwnctl/test/pwnctl.svc.test.int/deployment/seed/", Path.GetFileName(file)), true);

        Environment.SetEnvironmentVariable("PWNCTL_TEST_RUN", "true");
        Environment.SetEnvironmentVariable("PWNCTL_INSTALL_PATH", "/__w/pwnctl/pwnctl/test/pwnctl.svc.test.int/deployment");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__FilePath", "/__w/pwnctl/pwnctl/test/pwnctl.svc.test.int/deployment");
        PwnInfraContextInitializer.Setup();
    }

    [Fact]
    public async Task InitializeOperation_Test()
    {
        var context = new PwnctlDbContext();
        var scope = new ScopeAggregate("test_scope", "");
        scope.Definitions = new List<ScopeDefinitionAggregate>
        {
            new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.DomainRegex, "(^tesla\\.com$|.*\\.tesla\\.com$)")),
            new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.UrlRegex, "(.*:\\/\\/tsl\\.com\\/app\\/.*$)")),
            new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.CIDR, "172.16.17.0/24"))
        };
        var taskProfile = context.TaskProfiles.Include(p => p.TaskDefinitions).First();
        var policy = new Policy(taskProfile);
        var op = new Operation("test", OperationType.Monitor, policy, scope);
        context.Add(op);
        await context.SaveChangesAsync();
        var domain = new DomainName("tesla.com");
        var record = new AssetRecord(domain);
        record.Scope = scope.Definitions.First().Definition;
        context.Add(record);
        await context.SaveChangesAsync();

        var container = new ContainerBuilder()
            .WithImage("public.ecr.aws/i0m2p7r6/pwnctl:latest")
            .WithBindMount("/__w/pwnctl/pwnctl/test/pwnctl.svc.test.int/deployment", "/mnt/efs/")
            .WithEnvironment("PWNCTL_TEST_RUN", "true")
            .WithEnvironment("PWNCTL_INSTALL_PATH", "/mnt/efs")
            .WithEnvironment("PWNCTL_Logging__FilePath", "/mnt/efs")
            .WithEnvironment("PWNCTL_Logging__MinLevel", "Debug")
            .WithEnvironment("PWNCTL_Operation", op.Id.ToString())
            .Build();

        CancellationTokenSource _cts = new(TimeSpan.FromMinutes(10));

        await container.StartAsync(_cts.Token).ConfigureAwait(false);

        op = context.Operations.Find(op.Id);
        // TODO: InitiatedAt

        context.TaskEntries.Where(t => t.Definition.ShortName == ShortName.Create("domain_resolution")
                                    && t.Record.DomainName.Name == "tesla.com")
                            .First();
    }

    [Fact]
    public async Task TaskDefinition_Tests()
    {
        var context = new PwnctlDbContext();

        var assetMap = new Dictionary<AssetClass, Asset>
        {
            { AssetClass.Create(nameof(DomainName)), new DomainName("example.com") },
            { AssetClass.Create(nameof(NetworkHost)), new NetworkHost(IPAddress.Parse("127.0.0.1")) },
            { AssetClass.Create(nameof(NetworkSocket)), new NetworkSocket(new NetworkHost(IPAddress.Parse("127.0.0.1")), 443) }
        };

        var scope = new ScopeAggregate("test_scope", "");
        scope.Definitions = new List<ScopeDefinitionAggregate>
        {
            new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.DomainRegex, "(^tesla\\.com$|.*\\.tesla\\.com$)")),
            new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.UrlRegex, "(.*:\\/\\/tsl\\.com\\/app\\/.*$)")),
            new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.CIDR, "172.16.17.0/24"))
        };
        var taskProfile = context.TaskProfiles.Include(p => p.TaskDefinitions).First();
        var policy = new Policy(taskProfile);
        var op = new Operation("test", OperationType.Monitor, policy, scope);
        context.Add(op);
        await context.SaveChangesAsync();

        var taskDefinitions = await context.TaskDefinitions.ToListAsync();
        foreach (var definition in taskDefinitions.DistinctBy(d => d.ShortName.Value))
        {
            if (assetMap.ContainsKey(definition.SubjectClass))
            {
                var record = new AssetRecord(assetMap[definition.SubjectClass]);
                var task = new TaskEntry(op, definition, record);
                context.Add(task);
                context.SaveChanges();
                // TODO: enqueue tasks
            }
        }

        var container = new ContainerBuilder()
            .WithImage("public.ecr.aws/i0m2p7r6/pwnctl:latest")
            .WithBindMount("/tmp/", "/mnt/efs/")
            .WithEnvironment("PWNCTL_TEST_RUN", "true")
            .WithEnvironment("PWNCTL_INSTALL_PATH", "/mnt/efs")
            .WithEnvironment("PWNCTL_Logging__FilePath", "/mnt/efs")
            .WithEnvironment("PWNCTL_Logging__MinLevel", "Debug")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
            .Build();

        CancellationTokenSource _cts = new(TimeSpan.FromMinutes(10));

        //await container.StartAsync(_cts.Token).ConfigureAwait(false);
    }
}
