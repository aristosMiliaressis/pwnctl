namespace pwnctl.svc.test.integration;

using Xunit;
using DotNet.Testcontainers.Builders;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Scope.Enums;
using pwnctl.domain.Entities;
using pwnctl.app.Assets.Aggregates;
using Microsoft.EntityFrameworkCore;
using pwnctl.domain.BaseClasses;
using System.Net;
using pwnctl.domain.ValueObjects;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Common.ValueObjects;
using pwnctl.app;

public sealed class Tests
{
    private static readonly string _deploymentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory+"/../../../", "deployment");

    public Tests()
    {
        Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}/deployment/seed");

        foreach (var file in Directory.GetFiles("../../../../../src/core/pwnctl.infra/Persistence/seed"))
            File.Copy(file, Path.Combine("./deployment/seed/", Path.GetFileName(file)), true);

        Environment.SetEnvironmentVariable("PWNCTL_TEST_RUN", "true");
        Environment.SetEnvironmentVariable("PWNCTL_INSTALL_PATH", "deployment");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__FilePath", "deployment");
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
            .WithBindMount(_deploymentPath, "/mnt/efs/")
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

        // context.TaskEntries.Where(t => t.Definition.ShortName == ShortName.Create("domain_resolution")
        //                             && t.Record.DomainName.Name == "tesla.com")
        //                     .First();
    }

    // [Fact]
    // public async Task TaskDefinition_Tests()
    // {
    //     var context = new PwnctlDbContext();

    //     var assetMap = new Dictionary<AssetClass, Asset>
    //     {
    //         { AssetClass.Create(nameof(DomainName)), new DomainName("example.com") },
    //         { AssetClass.Create(nameof(NetworkHost)), new NetworkHost(IPAddress.Parse("127.0.0.1")) },
    //         { AssetClass.Create(nameof(NetworkSocket)), new NetworkSocket(new NetworkHost(IPAddress.Parse("127.0.0.1")), 443) }

    //     };

    //     var taskDefinitions = await context.TaskDefinitions.ToListAsync();
    //     foreach (var definition in taskDefinitions.DistinctBy(d => d.ShortName.Value))
    //     {
    //         if (assetMap.ContainsKey(definition.SubjectClass))
    //         {
    //             var record = new AssetRecord(assetMap[definition.SubjectClass]);
    //             var task = new TaskEntry(null, definition, record);
    //             context.Add(task);
    //             context.SaveChanges();
    //             // TODO: enqueue task
    //         }
    //     }

    //     var container = new ContainerBuilder()
    //         .WithImage("public.ecr.aws/i0m2p7r6/pwnctl:latest")
    //         .WithBindMount(_deploymentPath, "/mnt/efs/")
    //         .WithEnvironment("PWNCTL_TEST_RUN", "true")
    //         .WithEnvironment("PWNCTL_INSTALL_PATH", "/mnt/efs")
    //         .WithEnvironment("PWNCTL_Logging__FilePath", "/mnt/efs")
    //         .WithEnvironment("PWNCTL_Logging__MinLevel", "Debug")
    //         .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
    //         .Build();

    //     CancellationTokenSource _cts = new(TimeSpan.FromMinutes(10));

    //     await container.StartAsync(_cts.Token).ConfigureAwait(false);
    // }

    // [Fact]
    // public void Crawl_Test()
    // {
    //     // crawl test?
    // }
}
