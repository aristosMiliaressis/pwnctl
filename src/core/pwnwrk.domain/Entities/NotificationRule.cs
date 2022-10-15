using System.Text.RegularExpressions;
using pwnwrk.domain.Entities.Assets;
using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities
{
    public class NotificationRule : BaseEntity<int>
    {
        public string ShortName { get; private init; }
        public string Subject { get; private init; }
        public string Filter { get; private init; }
        public string Topic { get; private init; }
        public string Severity { get; private init; }

        public NotificationRule() { }
    }
}