namespace pwnctl.core.test.integration.Helpers;

using pwnctl.infra.Commands;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using pwnctl.app.Common.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

[Collection("Sequential")]
public class IntegrationTestBase
{
    protected CommandExecutor _cmdExecutor = new BashCommandExecutor();

    public IntegrationTestBase()
    {
        _cmdExecutor.ExecuteAsync("docker-compose -f ../../../docker-compose.ecs-local.yml up");
        Thread.Sleep(10000);

        // setup ambiant configuration context
        Environment.SetEnvironmentVariable("PWNCTL_Db__Host", "169.254.170.6:5432");
        Environment.SetEnvironmentVariable("PWNCTL_Db__Name", "pwnctl");
        Environment.SetEnvironmentVariable("PWNCTL_Db__Username", "pwnadmin");
        Environment.SetEnvironmentVariable("PWNCTL_Db__Password", "P@ssw0rd!");
        Environment.SetEnvironmentVariable("PWNCTL_LongLivedTaskQueue__Name", "dev-task-longlived.fifo");
        Environment.SetEnvironmentVariable("PWNCTL_ShortLivedTaskQueue__Name", "dev-task-shortlived.fifo");
        Environment.SetEnvironmentVariable("PWNCTL_OutputQueue__Name", "dev-output.fifo");
        PwnInfraContextInitializer.Setup();

        // migrate & seed database
        DatabaseInitializer.InitializeAsync(Assembly.GetExecutingAssembly(), null).Wait();
    }
}