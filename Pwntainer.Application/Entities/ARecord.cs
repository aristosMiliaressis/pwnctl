using System;
using System.Collections.Generic;
using System.Text;

namespace Pwntainer.Application.Entities
{
    public class ARecord : BaseAsset
    {
        public int Id { get; set; }
        public string DomainName { get; set; }
        public Domain Domain { get; set; }
        public string IP { get; set; }
        public Host Host { get; set; }
    }
}
