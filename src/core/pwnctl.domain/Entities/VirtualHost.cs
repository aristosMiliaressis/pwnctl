using pwnctl.domain.BaseClasses;

namespace pwnctl.domain.Entities
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

        public static bool TryParse(string assetText, out Asset asset)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
