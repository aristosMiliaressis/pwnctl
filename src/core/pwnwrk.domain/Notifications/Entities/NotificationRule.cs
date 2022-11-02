using System.Text.RegularExpressions;
using pwnwrk.domain.Assets.Entities;
using pwnwrk.domain.Common.BaseClasses;

namespace pwnwrk.domain.Notifications.Entities
{
    public sealed class NotificationRule : BaseEntity<int>
    {
        public string ShortName { get; private init; }
        public string Subject { get; private init; }
        public string Filter { get; private init; }
        public string Topic { get; private init; }
        public string Severity { get; private init; }

        public NotificationRule() { }
    }
}