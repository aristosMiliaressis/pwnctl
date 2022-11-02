using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Notifications.Entities;

namespace pwnwrk.infra.Notifications
{
    public sealed class NotificationRuleChecker
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
