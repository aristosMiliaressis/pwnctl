namespace pwnctl.app.test.unit;

using pwnctl.app.Assets;
using pwnctl.app.Assets.DTO;
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
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

[Collection("UnitTests")]
public sealed class TaggingTests
{
    public TaggingTests()
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
    public async Task Tagging_Tests()
    {
        var proc = new AssetProcessor();
        PwnctlDbContext context = new();
        AssetDbRepository repository = new(context);
        TaskDbRepository taskRepository = new(context);

        var exampleUrl = new AssetDTO
        {
            Asset = "https://example.com",
            Tags = new Dictionary<string, string>{
               {"Content-Type", "text/html"},
               {"Status", "200"},
               {"Server", "IIS"}
            }
        };

        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl), EntityFactory.TaskRecord.Id);

        var endpointRecord = context.AssetRecords
                                .Include(r => r.Tags)
                                .Include(r => r.HttpEndpoint)
                                .First(r => r.HttpEndpoint.Url == "https://example.com/");

        var ctTag = endpointRecord.Tags.First(t => t.Name == "content-type");
        Assert.Equal("text/html", ctTag.Value);

        var stTag = endpointRecord.Tags.First(t => t.Name == "status");
        Assert.Equal("200", stTag.Value);

        var srvTag = endpointRecord.Tags.First(t => t.Name == "server");
        Assert.Equal("IIS", srvTag.Value);

        exampleUrl.Tags = new Dictionary<string, string> {
            {"Server", "apache"},   // testing that existing tags don't get updated
            {"newTag", "whatever"}, // testing that new tags are added to existing assets
            {"emptyTag", ""},        // testing that empty tags are not added
            {"nullTag", ""}
        };

        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl), EntityFactory.TaskRecord.Id);

        endpointRecord = repository.ListHttpEndpointsAsync(0).Result.First(t => t.HttpEndpoint.Url == "https://example.com/");

        srvTag = endpointRecord.Tags.First(t => t.Name == "server");
        Assert.Equal("IIS", srvTag.Value);

        var newTag = endpointRecord.Tags.First(t => t.Name == "newtag");
        Assert.Equal("whatever", newTag.Value);

        Assert.Null(endpointRecord.Tags.FirstOrDefault(t => t.Name == "emptytag"));
        Assert.Null(endpointRecord.Tags.FirstOrDefault(t => t.Name == "nullTag"));

        ctTag = endpointRecord.Tags.First(t => t.Name == "content-type");
        Assert.Equal("text/html", ctTag.Value);

        stTag = endpointRecord.Tags.First(t => t.Name == "status");
        Assert.Equal("200", stTag.Value);

        var teslaUrl = new
        {
            asset = "https://iis.tesla.com",
            tags = new Dictionary<string, string>{
               {"Content-Type", "text/html"},
               {"Status", "200"},
               {"Protocol", "IIS"}
            }
        };

        // process same asset twice and make sure tasks are only assigned once
        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id);
        endpointRecord = repository.ListHttpEndpointsAsync(0).Result.First(ep => ep.HttpEndpoint.Url == "https://iis.tesla.com/");
        var tasks = context.TaskRecords.Include(t => t.Definition).Where(t => t.Record.Id == endpointRecord.Id).ToList();
        Assert.DoesNotContain(tasks.GroupBy(t => t.DefinitionId), g => g.Count() > 1);
        srvTag = endpointRecord.Tags.First(t => t.Name == "protocol");
        Assert.Equal("IIS", srvTag.Value);
        Assert.Contains(tasks, t => t.Definition.Name.Value == "shortname_scanner");

        var apacheTeslaUrl = new
        {
            asset = "https://apache.tesla.com",
            tags = new Dictionary<string, string>{
               {"Content-Type", "text/html"},
               {"Status", "200"},
               {"Server", "apache"},
               { "cors-misconfig", "true" }
            }
        };

        // test Tag filter
        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(apacheTeslaUrl), EntityFactory.TaskRecord.Id);
        endpointRecord = repository.ListHttpEndpointsAsync(0).Result.First(r => r.HttpEndpoint.Url == "https://apache.tesla.com/");

        tasks = context.TaskRecords.Include(t => t.Definition).Where(t => t.Record.Id == endpointRecord.Id).ToList();
        Assert.DoesNotContain(tasks, t => t.Definition.Name.Value == "shortname_scanner");

        var sshService = new
        {
            asset = "1.3.3.7:22",
            tags = new Dictionary<string, string>{
               {"ApplicationProtocol", "ssh"}
            }
        };

        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(sshService), EntityFactory.TaskRecord.Id);
        var service = context.NetworkSockets.First(ep => ep.Address == "tcp://1.3.3.7:22");

        await proc.ProcessAsync("https://test.tesla.com/", EntityFactory.TaskRecord.Id);
        await proc.ProcessAsync($$$"""{"Asset":"https://test.tesla.com/","Tags":{"Content-Type":"text/html"}}""", EntityFactory.TaskRecord.Id);

        endpointRecord = repository.ListHttpEndpointsAsync(0).Result.First(r => r.HttpEndpoint.Url == "https://test.tesla.com/");

        tasks = context.TaskRecords.Include(t => t.Definition).Where(t => t.Record.Id == endpointRecord.Id).ToList();
        Assert.Contains(tasks, t => t.Definition.Name.Value == "hakrawler");
    }
}
