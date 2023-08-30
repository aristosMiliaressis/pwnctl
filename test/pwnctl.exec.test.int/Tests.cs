namespace pwnctl.exec.test.integration;

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
                    .WithPortBinding(hostPort: 45432, 5432)
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
        Environment.SetEnvironmentVariable("PWNCTL_Db__Host", $"{_pwnctlDb.Hostname}:45432");
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
    public async Task Execute_Tasks_Happy_Path_Test()
    {
        // TODO: implement
    }

    [Fact]
    public async Task Execute_Tasks_Task_NotFound_Test()
    {
        // TODO: implement
    }

    [Fact]
    public async Task Execute_Tasks_Task_Invalid_State_Test()
    {
        // TODO: implement
    }

    [Fact]
    public async Task Execute_Tasks_Task_Timeout_Test()
    {
        // TODO: implement
    }
}