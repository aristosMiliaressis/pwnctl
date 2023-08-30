namespace pwnctl.proc.test.integration;

using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Tasks.Enums;
using pwnctl.infra.Aws;
using pwnctl.infra.Configuration;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;

using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Networks;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.Design.Serialization;
using Testcontainers.PostgreSql;
using Xunit;

public sealed class Tests
{
    private static readonly string _hostBasePath = EnvironmentVariables.GITHUB_ACTIONS
                ? "/home/runner/work/pwnctl/pwnctl/test/pwnctl.exec.test.int/deployment"
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "deployment");

    private static INetwork _pwnctlNetwork = new NetworkBuilder().Build();
    private static string _databaseHostname = $"postgres_{Guid.NewGuid()}";
    private static CredentialProfile _awsProfile = AwsConfigProvider.TryGetAWSProfile("default");
    private static AWSCredentials _awsCreds = AwsConfigProvider.TryGetAWSProfileCredentials("default");

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
                    .WithEnvironment("AWS_ACCESS_KEY_ID",  _awsCreds.GetCredentials().AccessKey)
                    .WithEnvironment("AWS_SECRET_ACCESS_KEY", _awsCreds.GetCredentials().SecretKey)
                    .WithEnvironment("AWS_DEFAULT_REGION", _awsProfile.Region.SystemName)
                    .WithEnvironment("PWNCTL_Logging__FilePath", "/mnt/efs")
                    .WithEnvironment("PWNCTL_Logging__MinLevel", "Debug")
                    .WithEnvironment("PWNCTL_INSTALL_PATH", "/mnt/efs")
                    .WithEnvironment("PWNCTL_Db__Host", $"{_databaseHostname}:5432")
                    .WithEnvironment("PWNCTL_Db__Name", "postgres")
                    .WithEnvironment("PWNCTL_Db__Username", "postgres")
                    .WithEnvironment("PWNCTL_Db__Password", "password")
                    .WithEnvironment("PWNCTL_TaskQueue__Name", "task-dev.fifo")
                    .WithEnvironment("PWNCTL_OutputQueue__Name", "output-dev.fifo");                    

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
        Environment.SetEnvironmentVariable("PWNCTL_TaskQueue__Name", "task-dev.fifo");
        Environment.SetEnvironmentVariable("PWNCTL_OutputQueue__Name", "output-dev.fifo");
        PwnInfraContextInitializer.SetupAsync().Wait();

        // migrate & seed database
        DatabaseInitializer.InitializeAsync(null).Wait();
        DatabaseInitializer.SeedAsync().Wait();
    }

    [Fact]
    public async Task InitializeOperation_Happy_Path_Test()
    {
        var op = EntityFactory.CreateOperation();

        CancellationTokenSource _cts = new(TimeSpan.FromMinutes(10));

        var pwnctlContainer = _pwnctlContainerBuilder
                    .WithEnvironment("PWNCTL_Operation", op.Id.ToString())
                    .Build();

        await pwnctlContainer.StartAsync(_cts.Token).ConfigureAwait(false);

        Thread.Sleep(20000);

        var context = new PwnctlDbContext();

        var def = context.TaskDefinitions.First(d => d.Name == ShortName.Create("domain_resolution"));
        var task = context.TaskRecords.Include(t => t.Definition).First(t => t.Definition.Name == ShortName.Create("domain_resolution"));
        Assert.NotEqual(DateTime.MinValue, task?.QueuedAt);
        Assert.Equal(TaskState.QUEUED, task?.State);

        op = context.Operations.Find(op.Id);

        Assert.NotEqual(DateTime.MinValue, op?.InitiatedAt);
    }

    [Fact]
    public async Task Process_Output_Batch_Happy_Path() 
    {
        // TODO: implement
    }
}
