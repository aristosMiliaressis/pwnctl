using pwnwrk.infra.Persistence;
using pwnwrk.infra;
using pwnwrk.domain.BaseClasses;
using pwnwrk.domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace pwnwrk.cli.Utilities
{
    public class NotificationRuleChecker
    {
        private readonly List<NotificationRule> _notificationRules;

        public NotificationRuleChecker()
        {
            PwnctlDbContext context = new();
            _notificationRules = context.NotificationRules.ToList();
        }

        public NotificationRule Check(BaseAsset asset)
        {
            return _notificationRules
                        .Where(r => r.Subject == asset.GetType().Name)
                        .FirstOrDefault(rule => CSharpScriptHelper.Evaluate(rule.Filter, asset));
        }        
    }
}
