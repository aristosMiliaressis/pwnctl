namespace pwnctl.infra;

using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.infra.Repositories;
using pwnctl.app.Assets;

using Microsoft.EntityFrameworkCore;

public static class AssetProcessorFactory
{
    public static AssetProcessor Create()
    {
        var context = new PwnctlDbContext();

        var assetRepository = new AssetDbRepository(context);

        var rules = context.NotificationRules.AsNoTracking().ToList(); // TODO: cache in-memory
        var outOfScopeTasks = context.TaskDefinitions.Where(d => d.MatchOutOfScope).AsNoTracking().ToList(); // TODO: cache in-memory

        return new AssetProcessor(assetRepository, rules, outOfScopeTasks);
    }
}
