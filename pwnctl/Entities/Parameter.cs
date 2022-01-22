using System;
using System.Collections.Generic;
using System.Text;

namespace pwnctl.Entities
{
    public class Parameter : BaseAsset, IAsset
    {
        private Parameter()
        {
            
        }

        public Endpoint Endpoint { get; set; }
        public int? EndpointId { get; set; }
        public Domain Domain { get; set; }
        public int? DomainId { get; set; }
        public VirtualHost VirtualHost { get; set; }
        public int? VirtualHostId { get; set; }

        public string Name { get; set; }
        public ParamType Type { get; set; }

        public enum ParamType
        {
            Query,
            Body,
            Cookie,
            Header,
        }
    }
}
