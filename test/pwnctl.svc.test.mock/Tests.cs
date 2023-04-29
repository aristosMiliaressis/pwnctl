namespace pwnctl.svc.test.mock;

using Xunit;
using DotNet.Testcontainers.Builders;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.app.Scope.Entities;

public sealed class Tests
{
    public Tests()
    {
        Directory.CreateDirectory("seed");

        foreach (var file in Directory.GetFiles("../../../../../src/core/pwnctl.infra/Persistence/seed"))
            File.Copy(file, Path.Combine("./seed/", Path.GetFileName(file)), true);
    }

    [Fact]
    public async Task InitializeOperation_Test()
    {
        Environment.SetEnvironmentVariable("PWNCTL_TEST_RUN", "true");
        Environment.SetEnvironmentVariable("PWNCTL_INSTALL_PATH", ".");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__FilePath", ".");
        PwnInfraContextInitializer.Setup();
        var context = new PwnctlDbContext();
        var scope = new ScopeAggregate("test_scope", "");
        //scope.Definitions
        var policy = new Policy(context.TaskProfiles.First());
        var op = new Operation("test", OperationType.Monitor, policy, scope);
        context.Add(op);
        await context.SaveChangesAsync();

        var container = new ContainerBuilder()
            .WithImage("public.ecr.aws/i0m2p7r6/pwnctl:latest")
            .WithBindMount(AppDomain.CurrentDomain.BaseDirectory, "/mnt/efs/")
            .WithEnvironment("PWNCTL_TEST_RUN", "true")
            .WithEnvironment("PWNCTL_INSTALL_PATH", "/mnt/efs")
            .WithEnvironment("PWNCTL_Logging__FilePath", "/mnt/efs")
            .WithEnvironment("PWNCTL_Logging__MinLevel", "Debug")
            .WithEnvironment("PWNCTL_Operation", "1")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
            .Build();

        CancellationTokenSource _cts = new(TimeSpan.FromMinutes(10));

        await container.StartAsync(_cts.Token).ConfigureAwait(false);

        // wait until container exits
        // check db for tasks
    }

    [Fact]
    public void TaskDefinition_Tests()
    {
        // test all task definitions! example.com, 127.0.0.1, 127.0.0.0/24
    }

    [Fact]
    public void Crawl_Test()
    {
        // crawl scope?
    }

    // TODO: create/delete test SQS queues?
    // validate discord configuration or terraform it yo self
}
