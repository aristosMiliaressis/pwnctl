using pwnctl.core.Entities;

namespace pwnctl.infra.Notifications
{
    public class NotificationSettings
    {
        public List<NotificationProviderSettings> Providers { get; set; }
        public List<NotificationRule> Rules { get; set; }
    }
}