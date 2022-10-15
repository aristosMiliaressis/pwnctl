using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities
{
    public class NotificationChannel : BaseEntity<int>
    {
        public string Name { get; private init; }
        public string Filter { get; private init; }

        public int ProviderId { get; private init; }
        public NotificationProviderSettings Provider { get; private init; }

        public NotificationChannel() { }
    }
}