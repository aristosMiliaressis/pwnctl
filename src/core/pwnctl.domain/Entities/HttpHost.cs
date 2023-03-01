using pwnctl.domain.BaseClasses;

namespace pwnctl.domain.Entities
{
    public sealed class HttpHost : Asset
    {
        public string Name { get; private init; }
        public NetworkSocket Socket { get; private init; }
        public string ServiceId { get; private init; }

        private HttpHost() {}

        public HttpHost(NetworkSocket address, string name)
        {
            Socket = address;
            Name = name;
        }

        public static Asset TryParse(string assetText)
        {
            return null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
