using System.Collections.Generic;

namespace pwnctl.Entities
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

        private Tag() {}
    }
}