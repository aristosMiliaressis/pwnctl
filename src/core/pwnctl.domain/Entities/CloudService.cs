using pwnctl.domain.Attributes;
using pwnctl.domain.BaseClasses;

namespace pwnctl.domain.Entities
{
    public sealed class CloudService : Asset
    {
        [UniquenessAttribute]
        public string Hostname { get; set; }
        public string Service { get; set; }
        public string Provider { get; set; }
        public Domain Domain { get; set; }
        public string DomainId { get; set; }

        private CloudService() { }

        public CloudService(Domain domain)
        {
            Hostname = domain.Name;
            Domain = domain;
        }

        public static bool TryParse(string assetText, out Asset[] assets)
        {
            assets = null;

            return false;
        }

        public override string ToString()
        {
            return Hostname;
        }
    }
}
