using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities
{
    public class NotificationProviderSettings : BaseEntity<int>
    {
        public string Name { get; set; }
        public List<NotificationChannel> Channels { get; set; }

        public NotificationProviderSettings() {}
    }
}