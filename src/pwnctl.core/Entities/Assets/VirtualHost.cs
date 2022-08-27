using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities.Assets
{
    public class VirtualHost : BaseAsset
    {
        public string Name { get; set; }
        public Service Service { get; set; }
        public int ServiceId { get; set; }

        private VirtualHost() {}

        public VirtualHost(Service service, string name)
        {
            Service = service;
            Name = name;
        }

        public override bool Matches(ScopeDefinition definition)
        {
            throw new NotImplementedException();
        }

        public static bool TryParse(string assetText, List<Tag> tags, out BaseAsset[] assets)
        {
            throw new NotImplementedException();
        }

    }
}
