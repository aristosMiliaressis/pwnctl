using System.Collections.Generic;
using pwnwrk.domain.Entities.Assets;
using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities
{
    public class Tag : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Host Host { get; set; }
        public string HostId { get; set; }
        
        public Service Service { get; set; }
        public string ServiceId { get; set; }

        public Endpoint Endpoint { get; set; }
        public string EndpointId { get; set; }
        
        public Domain Domain { get; set; }
        public string DomainId { get; set; }

        public DNSRecord DNSRecord { get; set; }
        public string DNSRecordId { get; set; }

        public NetRange NetRange { get; set; }
        public string NetRangeId { get; set; }

        public CloudService CloudService { get; set; }
        public string CloudServiceId { get; set; }

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