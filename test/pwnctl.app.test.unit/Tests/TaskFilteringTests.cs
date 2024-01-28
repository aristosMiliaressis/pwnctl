namespace pwnctl.app.test.unit;

using pwnctl.domain.Entities;
using pwnctl.app.Assets;
using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Common.Interfaces;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.infra.Commands;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using pwnctl.infra.Repositories;
using pwnctl.infra.Queueing;
using pwnctl.infra.Notifications;

using Xunit;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

[Collection("UnitTests")]
public sealed class TaskFilteringTests
{
    public TaskFilteringTests()
    {
        Environment.SetEnvironmentVariable("PWNCTL_USE_LOCAL_INTEGRATIONS", "true");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__MinLevel", "Warning");

        PwnInfraContextInitializer.Setup();
        DatabaseInitializer.InitializeAsync(Assembly.GetExecutingAssembly(), null).Wait();
        PwnInfraContextInitializer.Register<TaskQueueService, FakeTaskQueueService>();
        PwnInfraContextInitializer.Register<NotificationSender, StubNotificationSender>();
        PwnInfraContextInitializer.Register<CommandExecutor, BashCommandExecutor>();
    }

    [Fact]
    public async Task TaskFiltering_Tests()
    {
        PwnctlDbContext context = new();
        TaskDbRepository repository = new(context);
        AssetProcessor proc = new();

        await proc.ProcessAsync("172.16.17.0/24", EntityFactory.TaskRecord.Id);
        Assert.True(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("nmap_basic")));
        Assert.False(context.TaskRecords
                            .Include(t => t.Definition)
                            .Include(t => t.Record)
                            .Any(t => t.Record.NetworkRange.FirstAddress == "172.16.17.0"
                                   && t.Definition.Name == ShortName.Create("ffuf_common")));

        var exampleUrl = new
        {
            asset = "https://172.16.17.15/api/token",
            tags = new Dictionary<string, string>{
               {"Content-Type", "text/html"}
            }
        };

        // TaskDefinition.Filter fail test
        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl), EntityFactory.TaskRecord.Id);

        // Task.Command interpolation test
        var hakrawlerTask = context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Record)
                                    .First(t => t.Definition.Name == ShortName.Create("hakrawler"));
        Assert.Equal("hakrawler -plain -h 'User-Agent: Mozilla/5.0' https://172.16.17.15/api/token", hakrawlerTask.Command);

        // TaskDefinition.Filter pass test
        await proc.ProcessAsync("https://172.16.17.15/", EntityFactory.TaskRecord.Id);
        Assert.True(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("ffuf_common")));

        // Task added on existing asset
        exampleUrl = new
        {
            asset = "https://172.16.17.15/",
            tags = new Dictionary<string, string>{
               {"Protocol", "IIS"}
            }
        };
        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl), EntityFactory.TaskRecord.Id);
        Assert.True(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("shortname_scanner")));

        // multiple interpolation test
        await proc.ProcessAsync("sub.tesla.com", EntityFactory.TaskRecord.Id);
        var resolutionTask = context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Record)
                                    .First(t => t.Record.DomainName.Name == "sub.tesla.com"
                                             && t.Definition.Name == ShortName.Create("domain_resolution"));
        Assert.Equal("dig +short sub.tesla.com | awk '{print \"sub.tesla.com IN A \" $1}'", resolutionTask.Command);

        // blacklist test
        Assert.False(context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Record)
                                    .Any(t => t.Record.DomainName.Name == "sub.tesla.com"
                                           && t.Definition.Name == ShortName.Create("subfinder")));

        // Keyword test
        var cloudEnumTask = context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Record)
                                    .First(t => t.Definition.Name == ShortName.Create("cloud_enum"));
        Assert.Equal("cloud-enum.sh tesla", cloudEnumTask.Command);

        await proc.ProcessAsync("https://tesla.s3.amazonaws.com", EntityFactory.TaskRecord.Id);
        var record = context.AssetRecords.First(r => r.HttpEndpoint.Url == "https://tesla.s3.amazonaws.com/");

        var task = context.TaskRecords
                            .Include(t => t.Definition)
                            .First(t => t.Definition.Name == ShortName.Create("second_order_takeover"));
        Assert.NotNull(task);

        var outOfScope = new
        {
            asset = "https://outofscope.com/api/token",
            tags = new Dictionary<string, string>{
               {"Content-Type", "text/html"}
            }
        };

        // out of scope test
        // await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(outOfScope), EntityFactory.TaskRecord.Id);
        // var xx = context.AssetRecords
        //                 .Include(r => r.HttpEndpoint)
        //                 .Include(r => r.Tasks)
        //                 .Where(r => r.HttpEndpoint.Url == "https://outofscope.com/api/token")
        //                 .First();
        // Assert.Empty(xx.Tasks);


        // TODO: test TaskDefinition.CheckOutOfScope
        // TODO: test NotificationRule.CheckOutOfScope
    }
}
