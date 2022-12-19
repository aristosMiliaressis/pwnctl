namespace pwnctl.infra;

using pwnctl.infra.Persistence;
using pwnctl.infra.Repositories;
using Microsoft.EntityFrameworkCore;
using pwnctl.app.Assets;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.infra.Queues;

public static class AssetProcessorFactory
{
    public static AssetProcessor Create(TaskQueueService queueService = null)
    {
        queueService ??= TaskQueueServiceFactory.Create();

        var context = new PwnctlDbContext();

        var assetRepository = new AssetDbRepository();

        var definitions = context.TaskDefinitions.ToList();

        var rules = context.NotificationRules.AsNoTracking().ToList();

        return new AssetProcessor(queueService, assetRepository, definitions, rules);
    }
}
