using pwnctl.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace pwnctl.Entities
{
    public class ServiceTag : BaseAsset
    {
        public int ServiceTagId { get; set; }

        public Tag Tag { get; set; }
    }
}
