using pwnctl.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace pwnctl.Entities
{
    public class EndpointTag : BaseAsset
    {
        public int EndpointId { get; set; }
        public Endpoint Endpoint { get; set; }

        public Tag Tag { get; set; }
    }
}
