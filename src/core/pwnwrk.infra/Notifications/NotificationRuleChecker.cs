using pwnwrk.infra.Persistence;
using pwnwrk.infra;
using pwnwrk.domain.BaseClasses;
using pwnwrk.domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace pwnwrk.infra.Notifications
{
    public class NotificationRuleChecker
    {
        private readonly List<NotificationRule> _notificationRules;

        public NotificationRuleChecker(List<NotificationRule> notificationRules)
        {
            _notificationRules = notificationRules;
        }

        public NotificationRule Check(BaseAsset asset)
        {
            return _notificationRules
                        .Where(r => r.Subject == asset.GetType().Name)
                        .FirstOrDefault(rule => CSharpScriptHelper.Evaluate(rule.Filter, asset));
        }        
    }
}
