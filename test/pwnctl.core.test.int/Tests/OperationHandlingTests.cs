namespace pwnctl.core.test.integration;

using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Operations;
using pwnctl.infra.Persistence;
using pwnctl.infra.Repositories;
using pwnctl.infra.Scheduling;
using pwnctl.core.test.integration.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using pwnctl.app.Operations.Enums;

public sealed class OperationHandlingTests : IntegrationTestBase
{
    private static OperationManager _opManager = new(new OperationDbRepository(), 
                                                    new TaskDbRepository(),
                                                    new EventBridgeClient());

    [Fact]
    public async Task Initialize_Monitor_Operation()
    {
        var context = new PwnctlDbContext();

        var op = context.Operations.Where(o => o.Type == OperationType.Monitor).FirstOrDefault();
        if (op is null)
        {
            op = EntityFactory.EnsureMonitorOperationCreated();
        }

        Assert.Equal(OperationState.Pending, op?.State);
        await _opManager.TryHandleAsync(op.Id);

        context = new PwnctlDbContext();
        op = context.Operations.Find(op.Id);

        Assert.NotEqual(DateTime.MinValue, op?.InitiatedAt);
        Assert.Equal(OperationState.Ongoing, op?.State);

        // TODO: check that tasks were generated
        // var def = context.TaskDefinitions.First(d => d.Name == ShortName.Create("domain_resolution"));
        // var task = context.TaskRecords.Include(t => t.Definition).First(t => t.Definition.Name == ShortName.Create("domain_resolution"));
        // Assert.NotEqual(DateTime.MinValue, task?.QueuedAt);
        //Assert.Equal(TaskState.QUEUED, task?.State);
    }

    // [Fact]
    // public async Task Initialize_Scan_Operation()
    // {
    //     return Task.CompletedTask;  // TODO: implement
    // }

    // [Fact]
    // public async Task Terminate_Operation()
    // {
    //     return Task.CompletedTask;  // TODO: implement
    // }

    // [Fact]
    // public async Task Transition_Operation_Phase()
    // {
    //     return Task.CompletedTask;  // TODO: implement
    // }
}
