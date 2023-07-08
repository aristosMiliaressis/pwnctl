namespace pwnctl.infra.Repositories;

using System.Collections.Generic;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.infra.Persistence;

public sealed class NotificationDbRepository : NotificationRepository
{
    private PwnctlDbContext _context = new PwnctlDbContext();

    public NotificationDbRepository(PwnctlDbContext context)
    {
        _context = context;
    }

    public List<NotificationRule> ListRules()
    {
        return _context.NotificationRules.ToList();
    }
}
