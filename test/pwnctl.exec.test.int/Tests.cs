namespace pwnctl.exec.test.integration;

using pwnctl.infra.Commands;
using pwnctl.infra.Configuration;
using pwnctl.infra.Queueing;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using pwnctl.infra.Notifications;
using pwnctl.domain.Entities;
using pwnctl.app.Common.Interfaces;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.app.Assets;
using pwnctl.app.Assets.Entities;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Tasks.Enums;
using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Queueing.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Diagnostics.Contracts;
using System.ComponentModel.Design.Serialization;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class Tests
{
    private static readonly string _hostBasePath = EnvironmentVariables.IN_GHA
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
                    .WithBindMount($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.aws/", "/root/.aws/")
                    .WithEnvironment("PWNCTL_INSTALL_PATH", "/mnt/efs")
                    .WithEnvironment("PWNCTL_Logging__FilePath", "/mnt/efs")
                    .WithEnvironment("PWNCTL_Logging__MinLevel", "Debug")
                    .WithEnvironment("PWNCTL_Db__Host", $"{_databaseHostname}:5432")
                    .WithEnvironment("PWNCTL_Db__Name", "postgres")
                    .WithEnvironment("PWNCTL_Db__Username", "postgres")
                    .WithEnvironment("PWNCTL_Db__Password", "password")
                    .WithEnvironment("PWNCTL_TaskQueue__Name", "task-dev.fifo")
                    .WithEnvironment("PWNCTL_TaskQueue__VisibilityTimeout", "1200")
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
        Environment.SetEnvironmentVariable("PWNCTL_INSTALL_PATH", _hostBasePath);
        Environment.SetEnvironmentVariable("PWNCTL_Db__Host", $"{_pwnctlDb.Hostname}:45432");
        Environment.SetEnvironmentVariable("PWNCTL_Db__Name", "postgres");
        Environment.SetEnvironmentVariable("PWNCTL_Db__Username", "postgres");
        Environment.SetEnvironmentVariable("PWNCTL_Db__Password", "password");
        Environment.SetEnvironmentVariable("PWNCTL_TaskQueue__Name", "task-dev.fifo");
        Environment.SetEnvironmentVariable("PWNCTL_OutputQueue__Name", "output-dev.fifo");
        PwnInfraContextInitializer.Setup();

        // migrate & seed database
        DatabaseInitializer.InitializeAsync(null).Wait();
        DatabaseInitializer.SeedAsync().Wait();
        PwnInfraContextInitializer.Register<TaskQueueService, SQSTaskQueueService>();
        PwnInfraContextInitializer.Register<NotificationSender, StubNotificationSender>();
        PwnInfraContextInitializer.Register<CommandExecutor, StubCommandExecutor>();
    }

    [Fact]
    public async Task Execute_Tasks_Happy_Path_Test()
    {
        var context = new PwnctlDbContext();
        var taskQueue = new SQSTaskQueueService();
        var processor = new AssetProcessor();

        var op = EntityFactory.CreateOperation();
        var domain = new DomainName("tesla.com");
        var asset = new AssetRecord(domain);

        var subEnum = context.TaskDefinitions.First(d => d.Name == ShortName.Create("sub_enum"));
        var task = new TaskRecord(op, subEnum, asset);
        context.Add(asset);
        context.Add(task);
        context.SaveChanges();
        var taskDTO = new PendingTaskDTO(task);
        taskDTO.Command = Guid.NewGuid().ToString();
        await taskQueue.EnqueueAsync(taskDTO);

        CancellationTokenSource _cts = new(TimeSpan.FromMinutes(10));
        var pwnctlContainer = _pwnctlContainerBuilder.Build();

        await pwnctlContainer.StartAsync(_cts.Token).ConfigureAwait(false);

        Thread.Sleep(20000);

        context = new PwnctlDbContext();
        task = context.TaskRecords.First(t => t.Id == task.Id);
        Assert.Equal(TaskState.FINISHED, task.State);
        
        // TODO: check that output queue was populated
    }

    [Fact]
    public Task Execute_Tasks_Task_NotFound_Test()
    {
        return Task.CompletedTask;  // TODO: implement
    }

    [Fact]
    public Task Execute_Tasks_Task_Invalid_State_Test()
    {
        return Task.CompletedTask;  // TODO: implement
    }

    [Fact]
    public Task Execute_Tasks_Task_Timeout_Test()
    {
        return Task.CompletedTask;  // TODO: implement
    }
}