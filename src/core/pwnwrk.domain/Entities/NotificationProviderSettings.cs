using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities
{
    public class NotificationProviderSettings : BaseEntity<int>
    {
        public string Name { get; set; }
        public List<NotificationChannel> Channels { get; set; }

        public NotificationProviderSettings() {}
    }
}