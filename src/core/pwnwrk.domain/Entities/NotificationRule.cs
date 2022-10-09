using System.Text.RegularExpressions;
using pwnwrk.domain.Entities.Assets;
using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities
{
    public class NotificationRule : BaseEntity<int>
    {
        public string ShortName { get; set; }
        public string Subject { get; set; }
        public string Filter { get; set; }
        public string Topic { get; set; }
        public string Severity { get; set; }

        public NotificationRule() { }
    }
}