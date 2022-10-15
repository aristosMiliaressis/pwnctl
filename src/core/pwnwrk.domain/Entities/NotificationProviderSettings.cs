using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities
{
    public class NotificationProviderSettings : BaseEntity<int>
    {
        public string Name { get; private init; }
        public List<NotificationChannel> Channels { get; private init; }

        public NotificationProviderSettings() {}
    }
}