using System;
using System.Collections.Generic;
using System.Text;

namespace pwnctl.Entities
{
    public class VirtualHost : BaseAsset, IAsset
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
    }
}
