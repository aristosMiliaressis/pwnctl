using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities
{
    public class NotificationChannel : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Filter { get; set; }

        public int ProviderId { get; set; }
        public NotificationProviderSettings Provider { get; set; }

        public NotificationChannel() { }
    }
}