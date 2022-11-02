using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Common.Entities;
using pwnwrk.domain.Targets.Entities;

namespace pwnwrk.domain.Assets.Entities
{
    public class VirtualHost : BaseAsset
    {
        public string Name { get; private init; }
        public Service Service { get; private init; }
        public string ServiceId { get; private init; }

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

        public override string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}
