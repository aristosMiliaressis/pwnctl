using pwnwrk.domain.Entities;

namespace pwnwrk.infra.Notifications
{
    public class NotificationSettings
    {
        public List<NotificationProviderSettings> Providers { get; set; }
        public List<NotificationRule> Rules { get; set; }
    }
}