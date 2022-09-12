using System.Collections.Generic;
using pwnctl.core.Entities.Assets;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Host Host { get; set; }
        public int? HostId { get; set; }
        
        public Service Service { get; set; }
        public int? ServiceId { get; set; }

        public Endpoint Endpoint { get; set; }
        public int? EndpointId { get; set; }
        
        public Domain Domain { get; set; }
        public int? DomainId { get; set; }

        public DNSRecord DNSRecord { get; set; }
        public int? DNSRecordId { get; set; }

        public NetRange NetRange { get; set; }
        public int? NetRangeId { get; set; }

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