using System;
using System.Collections.Generic;
using System.Text;

namespace Pwntainer.Application.Entities
{
    public class ARecord : BaseAsset
    {
        public int DomainId { get; set; }
        public Domain Domain { get; set; }
        public int HostId { get; set; }
        public Host Host { get; set; }
    }
}
