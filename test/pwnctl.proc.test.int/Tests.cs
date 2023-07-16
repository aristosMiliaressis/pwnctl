namespace pwnctl.proc.test.integration;

using pwnctl.infra.Configuration;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using DotNet.Testcontainers.Builders;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using DotNet.Testcontainers.Configurations;
using Testcontainers.PostgreSql;
using DotNet.Testcontainers.Networks;
using pwnctl.app.Common.ValueObjects;
using Microsoft.EntityFrameworkCore;
using pwnctl.app.Tasks.Enums;

public sealed class Tests
{
    private static readonly string _hostBasePath = EnvironmentVariables.GITHUB_ACTIONS
                ? "/home/runner/work/pwnctl/pwnctl/test/pwnctl.exec.test.int/deployment"
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "deployment");

    private static INetwork _pwnctlNetwork = new NetworkBuilder().Build();
    private static string _databaseHostname = $"postgres_{Guid.NewGuid()}";

    private static PostgreSqlContainer _pwnctlDb = new PostgreSqlBuilder()
                    .WithNetwork(_pwnctlNetwork)
                    .WithNetworkAliases(_databaseHostname)
                    .WithPortBinding(hostPort: 55432, 5432)
                    .WithDatabase("postgres")
                    .WithUsername("postgres")
                    .WithPassword("password")
                    .Build();
    
    private static ContainerBuilder _pwnctlContainerBuilder = new ContainerBuilder()
                    .WithImage($"{Environment.GetEnvironmentVariable("ECR_REGISTRY_URI")}:untested")
                    .WithNetwork(_pwnctlNetwork)
                    .WithBindMount(_hostBasePath, "/mnt/efs", AccessMode.ReadWrite)
                    .WithEnvironment("PWNCTL_TEST_RUN", "true")
                    .WithEnvironment("PWNCTL_INSTALL_PATH", "/mnt/efs")
                    .WithEnvironment("PWNCTL_Db__Host", $"{_databaseHostname}:5432")
                    .WithEnvironment("PWNCTL_Db__Name", "postgres")
                    .WithEnvironment("PWNCTL_Db__Username", "postgres")
                    .WithEnvironment("PWNCTL_Db__Password", "password");                    

    public Tests()
    {       
        // setup mount directory
        Directory.CreateDirectory($"{_hostBasePath}/seed");
        foreach (var file in Directory.GetFiles("../../../../../src/core/pwnctl.infra/Persistence/seed"))
            File.Copy(file, Path.Combine($"{_hostBasePath}/seed/", Path.GetFileName(file)), true);

        // setup docker network
        _pwnctlNetwork.CreateAsync().Wait();

        // start database
        _pwnctlDb.StartAsync().Wait();

        // setup ambiant configuration context
        Environment.SetEnvironmentVariable("PWNCTL_TEST_RUN", "true");
        Environment.SetEnvironmentVariable("PWNCTL_INSTALL_PATH", _hostBasePath);
        Environment.SetEnvironmentVariable("PWNCTL_Db__Host", $"{_pwnctlDb.Hostname}:55432");
        Environment.SetEnvironmentVariable("PWNCTL_Db__Name", "postgres");
        Environment.SetEnvironmentVariable("PWNCTL_Db__Username", "postgres");
        Environment.SetEnvironmentVariable("PWNCTL_Db__Password", "password");
        PwnInfraContextInitializer.SetupAsync().Wait();

        // migrate & seed database
        DatabaseInitializer.InitializeAsync(null).Wait();
        DatabaseInitializer.SeedAsync().Wait();
    }

    [Fact]
    public async Task InitializeOperation_Test()
    {
        var op = EntityFactory.CreateOperation();

        CancellationTokenSource _cts = new(TimeSpan.FromMinutes(10));

        var pwnctlContainer = _pwnctlContainerBuilder
                    .WithEnvironment("PWNCTL_Operation", "1")
                    .Build();

        await pwnctlContainer.StartAsync(_cts.Token).ConfigureAwait(false);

        Thread.Sleep(10000);

        var context = new PwnctlDbContext();

        // TODO: test integration with sqs instead of a faked queue

        var task = context.TaskRecords.Include(t => t.Definition).First(t => t.Definition.Name == ShortName.Create("domain_resolution"));
        Assert.NotEqual(DateTime.MinValue, task?.QueuedAt);
        Assert.Equal(TaskState.QUEUED, task?.State);

        op = context.Operations.Find(op.Id);

        Assert.NotEqual(DateTime.MinValue, op?.InitiatedAt);
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

    //     var scope = new ScopeAggregate("test_scope", "");
    //     scope.Definitions = new List<ScopeDefinitionAggregate>
    //     {
    //         new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.DomainRegex, "(^tesla\\.com$|.*\\.tesla\\.com$)")),
    //         new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.UrlRegex, "(.*:\\/\\/tsl\\.com\\/app\\/.*$)")),
    //         new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.CIDR, "172.16.17.0/24"))
    //     };
    //     var taskProfile = context.TaskProfiles.Include(p => p.TaskDefinitions).First();
    //     var policy = new Policy(taskProfile);
    //     var op = new Operation("test", OperationType.Monitor, policy, scope);
    //     context.Add(op);
    //     await context.SaveChangesAsync();

    //     var taskDefinitions = await context.TaskDefinitions.ToListAsync();
    //     foreach (var definition in taskDefinitions.DistinctBy(d => d.Name.Value))
    //     {
    //         if (assetMap.ContainsKey(definition.Subject))
    //         {
    //             var record = new AssetRecord(assetMap[definition.Subject]);
    //             var task = new TaskRecord(op, definition, record);
    //             // record.Tasks.Add(task);
    //             // context.Entry(task.Record).State = EntityState.Added;
    //             // context.Entry(task).State = EntityState.Added;
    //             // await context.SaveChangesAsync();
    //             // TODO: enqueue tasks
    //         }
    //     }

    //     var container = _pwnctlContainerBuilder.Build();

    //     CancellationTokenSource _cts = new(TimeSpan.FromMinutes(10));

    //     await container.StartAsync(_cts.Token).ConfigureAwait(false);
    //     Thread.Sleep(10000);

    //     // TODO: test if execution was successful?
    // }
}
