using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;

namespace pwnctl.domain.Entities
{
    public sealed class CloudService : Asset
    {
        [EqualityComponent]
        public string Hostname { get; set; }
        public string Service { get; set; }
        public string Provider { get; set; }
        public Domain Domain { get; set; }
        public string DomainId { get; set; }

        public CloudService() { }

        public CloudService(Domain domain)
        {
            Hostname = domain.Name;
            Domain = domain;
        }

        public static bool TryParse(string assetText, out Asset asset)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Hostname;
        }
    }
}
