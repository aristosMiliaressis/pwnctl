namespace pwnctl.svc.test.integration;

using pwnctl.domain.BaseClasses;
using pwnctl.domain.Entities;
using pwnctl.domain.ValueObjects;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Scope.Enums;
using pwnctl.app.Tasks.Entities;
using pwnctl.infra.Configuration;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using pwnctl.infra.Queueing;
using pwnctl.app.Queueing.DTO;
using pwnctl.app;

public sealed class Tests
{
    private static readonly string _hostBasePath = EnvironmentVariables.GITHUB_ACTIONS
                ? "/__w/pwnctl/pwnctl/test/pwnctl.svc.test.int/deployment"
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "deployment");

    private static readonly string _containerBasePath = EnvironmentVariables.GITHUB_ACTIONS
                ? "/tmp/pwnctl/test/pwnctl.svc.test.int/deployment"
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "deployment");

    public Tests()
    {
        Directory.CreateDirectory($"{_hostBasePath}/seed");

        foreach (var file in Directory.GetFiles("../../../../../src/core/pwnctl.infra/Persistence/seed"))
            File.Copy(file, Path.Combine($"{_hostBasePath}/seed/", Path.GetFileName(file)), true);

        Environment.SetEnvironmentVariable("PWNCTL_TEST_RUN", "true");
        Environment.SetEnvironmentVariable("PWNCTL_INSTALL_PATH", _hostBasePath);
        Environment.SetEnvironmentVariable("PWNCTL_Logging__FilePath", _hostBasePath);
        PwnInfraContextInitializer.SetupAsync().Wait();
    }

    [Fact]
    public async Task InitializeOperation_Test()
    {
        var op = EntityFactory.CreateOperation();

        var container = new ContainerBuilder()
            .WithImage("public.ecr.aws/i0m2p7r6/pwnctl:latest")
            .WithBindMount(_containerBasePath, "/mnt/efs/")
            .WithEnvironment("PWNCTL_TEST_RUN", "true")
            .WithEnvironment("PWNCTL_INSTALL_PATH", "/mnt/efs")
            .WithEnvironment("PWNCTL_Logging__FilePath", "/mnt/efs")
            .WithEnvironment("PWNCTL_Logging__MinLevel", "Debug")
            .WithEnvironment("PWNCTL_Operation", op.Id.ToString())
            .Build();

        CancellationTokenSource _cts = new(TimeSpan.FromMinutes(10));

        await container.StartAsync(_cts.Token).ConfigureAwait(false);

        Thread.Sleep(10000);

        var queue = new FakeTaskQueueService();
        var context = new PwnctlDbContext();
        var tasks = new List<TaskEntry>();
        while (true)
        {
            var taskDTO = await queue.ReceiveAsync<PendingTaskDTO>();
            if (taskDTO == default(PendingTaskDTO))
                break;

            var taskEntry = context.TaskEntries.Include(t => t.Definition).First(t => t.Id == taskDTO.TaskId);
            tasks.Add(taskEntry);

            await queue.DequeueAsync(taskDTO);
        }

        Assert.Contains("domain_resolution", tasks.Select(t => t.Definition.ShortName.Value));
        Assert.Contains("httpx", tasks.Select(t => t.Definition.ShortName.Value));

        op = context.Operations.Find(op.Id);

        Assert.NotEqual(DateTime.MinValue, op?.InitiatedAt);
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
                // record.Tasks.Add(task);
                // context.Entry(task.Record).State = EntityState.Added;
                // context.Entry(task).State = EntityState.Added;
                // await context.SaveChangesAsync();
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

        // await container.StartAsync(_cts.Token).ConfigureAwait(false);
        // Thread.Sleep(10000);

        // TODO: test if execution was successful?
    }
}
