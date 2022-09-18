using System.Text.RegularExpressions;
using pwnctl.core.Entities.Assets;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities
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