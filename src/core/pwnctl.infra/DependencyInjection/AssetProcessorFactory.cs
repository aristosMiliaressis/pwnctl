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
        var taskRepo = new TaskDbRepository(context);
        var notificationRepo = new NotificationDbRepository(context);

        var taskQueueService = TaskQueueServiceFactory.Create();

        return new AssetProcessor(assetRepo, taskQueueService, taskRepo, notificationRepo);
    }
}
