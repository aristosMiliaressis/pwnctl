namespace pwnctl.infra.Repositories;

using System.Collections.Generic;
using pwnctl.app.Common;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.domain.BaseClasses;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.infra.Persistence.IdGenerators;

public sealed class NotificationDbRepository : NotificationRepository
{
    private PwnctlDbContext _context;

    public NotificationDbRepository(PwnctlDbContext context)
    {
        _context = context;
    }

    public List<NotificationRule> ListRules()
    {
        return _context.NotificationRules.ToList();
    }

    public async Task<Notification> FindNotificationAsync(Asset asset, NotificationRule rule)
    {
        var recordId = UUIDv5ValueGenerator.GenerateByString(asset.ToString());

        var lambda = ExpressionTreeBuilder.BuildNotificationMatchingLambda(recordId, rule.Id);

        return await _context.FirstNotTrackedFromLambdaAsync<Notification>(lambda);
    }
}
