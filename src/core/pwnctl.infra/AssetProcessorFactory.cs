namespace pwnctl.infra;

using pwnctl.infra.Persistence;
using pwnctl.infra.Repositories;
using pwnctl.infra.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using pwnctl.app.Assets;
using pwnctl.app.Tasks.Interfaces;

public static class AssetProcessorFactory
{
    public static AssetProcessor Create(TaskQueueService queueService)
    {
        var context = new PwnctlDbContext();

        var assetRepository = new AssetDbRepository();

        var programs = context.ListPrograms();

        var definitions = context.TaskDefinitions.ToList();

        var rules = context.NotificationRules.AsNoTracking().ToList();

        return new AssetProcessor(queueService, assetRepository, definitions, rules, programs);
    }
}
