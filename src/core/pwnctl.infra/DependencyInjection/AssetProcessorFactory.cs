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

        var definitions = context.TaskDefinitions.ToList();

        var rules = context.NotificationRules.AsNoTracking().ToList();
        
        var programs = context.ListPrograms();
        
        return new AssetProcessor(assetRepository, programs, definitions, rules);
    }
}
