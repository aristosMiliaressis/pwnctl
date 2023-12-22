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
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;
using Xunit;
using System.Reflection;

public sealed class Tests
{
    private static readonly string _hostBasePath = EnvironmentVariables.IS_GHA
                ? "/home/runner/work/pwnctl/pwnctl/test/pwnctl.exec.test.int/App_Data"
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");

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
                    .WithImage($"{Environment.GetEnvironmentVariable("UNTESTED_IMAGE")}")
                    .WithNetwork(_pwnctlNetwork)
                    .WithBindMount($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.aws/", "/root/.aws/")
                    .WithEnvironment("PWNCTL_Logging__MinLevel", "Debug")
                    .WithEnvironment("PWNCTL_Db__Host", $"{_databaseHostname}:5432")
                    .WithEnvironment("PWNCTL_Db__Name", "postgres")
                    .WithEnvironment("PWNCTL_Db__Username", "postgres")
                    .WithEnvironment("PWNCTL_Db__Password", "password")
                    .WithEnvironment("PWNCTL_LongLivedTaskQueue__Name", "dev-task-longlived.fifo")
                    .WithEnvironment("PWNCTL_LongLivedTaskQueue__VisibilityTimeout", "1200")
                    .WithEnvironment("PWNCTL_ShortLivedTaskQueue__Name", "dev-task-shortlived.fifo")
                    .WithEnvironment("PWNCTL_ShortLivedTaskQueue__VisibilityTimeout", "1200")
                    .WithEnvironment("PWNCTL_OutputQueue__Name", "dev-output.fifo")
                    .WithEnvironment("PWNCTL_Worker__MaxTaskTimeout", "7200");

    public Tests()
    {
        // setup mount directory
        Directory.CreateDirectory($"{_hostBasePath}/seed");
        foreach (var file in Directory.GetFiles("../../../../../src/pwnctl.api/App_Data/seed"))
            File.Copy(file, Path.Combine($"{_hostBasePath}/seed/", Path.GetFileName(file)), true);

        // setup docker network
        _pwnctlNetwork.CreateAsync().Wait();

        // start database
        _pwnctlDb.StartAsync().Wait();

        // setup ambiant configuration context
        Environment.SetEnvironmentVariable("PWNCTL_Db__Host", $"{_pwnctlDb.Hostname}:45432");
        Environment.SetEnvironmentVariable("PWNCTL_Db__Name", "postgres");
        Environment.SetEnvironmentVariable("PWNCTL_Db__Username", "postgres");
        Environment.SetEnvironmentVariable("PWNCTL_Db__Password", "password");
        Environment.SetEnvironmentVariable("PWNCTL_LongLivedTaskQueue__Name", "dev-task-longlived.fifo");
        Environment.SetEnvironmentVariable("PWNCTL_ShortLivedTaskQueue__Name", "dev-task-shortlived.fifo");
        Environment.SetEnvironmentVariable("PWNCTL_OutputQueue__Name", "output-dev.fifo");
        PwnInfraContextInitializer.Setup();

        // migrate & seed database
        DatabaseInitializer.InitializeAsync(Assembly.GetExecutingAssembly(), null).Wait();
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

        await taskQueue.Purge<LongLivedTaskDTO>();
        await taskQueue.Purge<ShortLivedTaskDTO>();

        var op = EntityFactory.CreateOperation();
        var domain = DomainName.TryParse("tesla.com").Value;
        var asset = new AssetRecord(domain);

        var subEnum = context.TaskDefinitions.First(d => d.Name == ShortName.Create("sub_enum"));
        var task = new TaskRecord(op, subEnum, asset);
        context.Add(asset);
        context.Add(task);
        context.SaveChanges();
        var taskDTO = new LongLivedTaskDTO(task);
        taskDTO.Command = Guid.NewGuid().ToString();
        await taskQueue.EnqueueAsync(taskDTO);

        CancellationTokenSource _cts = new(TimeSpan.FromMinutes(10));
        var pwnctlContainer = _pwnctlContainerBuilder.Build();

        await pwnctlContainer.StartAsync(_cts.Token).ConfigureAwait(false);

        Thread.Sleep(20000);

        var logs = await pwnctlContainer.GetLogsAsync(default, DateTime.Now);
        Console.WriteLine("STDOUT>>>  " + logs.Stdout);
        Console.WriteLine("STDERR>>>  " + logs.Stderr);

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