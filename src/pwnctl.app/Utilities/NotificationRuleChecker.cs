using pwnctl.infra.Persistence;
using pwnctl.infra;
using pwnctl.core.BaseClasses;
using pwnctl.core.Entities;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.app.Utilities
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
