using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.DTO;
using pwnwrk.domain.Common.Entities;
using pwnwrk.domain.Targets.Entities;

namespace pwnwrk.domain.Assets.Entities
{
    public sealed class VirtualHost : Asset
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

        internal override bool Matches(ScopeDefinition definition)
        {
            throw new NotImplementedException();
        }

        public static bool TryParse(string assetText, List<Tag> tags, out Asset[] assets)
        {
            throw new NotImplementedException();
        }

        public override AssetDTO ToDTO()
        {
            throw new NotImplementedException();
        }
    }
}
