namespace pwnctl.infra;

using pwnctl.infra.Persistence;
using pwnctl.infra.Repositories;
using pwnctl.app.Assets;

using Microsoft.EntityFrameworkCore;
using pwnctl.infra.Queueing;

public static class AssetProcessorFactory
{
    public static AssetProcessor Create()
    {
        var context = new PwnctlDbContext();

        var assetRepo = new AssetDbRepository(context);
        var taskRepo = new TaskDbRepository();

        var taskQueueService = TaskQueueServiceFactory.Create();

        // TODO: maybe cache in-memory?
        var rules = context.NotificationRules.AsNoTracking().ToList();
        var outOfScopeTasks = context.TaskDefinitions
                                    .Where(d => d.MatchOutOfScope)
                                    .AsNoTracking()
                                    .ToList();

        return new AssetProcessor(assetRepo, taskRepo, taskQueueService, rules, outOfScopeTasks);
    }
}
