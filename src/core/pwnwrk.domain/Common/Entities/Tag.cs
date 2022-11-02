using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.Entities;
using pwnwrk.domain.Common.BaseClasses;

namespace pwnwrk.domain.Common.Entities
{
    public class Tag : BaseEntity<int>
    {
        public string Name { get; private init; }
        public string Value { get; private init; }

        public Host Host { get; private init; }
        public string HostId { get; private init; }
        
        public Service Service { get; private init; }
        public string ServiceId { get; private init; }

        public Endpoint Endpoint { get; private init; }
        public string EndpointId { get; private init; }
        
        public Domain Domain { get; private init; }
        public string DomainId { get; private init; }

        public DNSRecord DNSRecord { get; private init; }
        public string DNSRecordId { get; private init; }

        public NetRange NetRange { get; private init; }
        public string NetRangeId { get; private init; }

        public CloudService CloudService { get; private init; }
        public string CloudServiceId { get; private init; }

        private Tag() {}

        public Tag(string name, string value)
        {
            Name = name.ToLower();
            Value = value;
        }

        public void SetAsset(BaseAsset asset)
        {
            GetType().GetProperty(asset.GetType().Name).SetValue(this, asset);
        }
    }
}