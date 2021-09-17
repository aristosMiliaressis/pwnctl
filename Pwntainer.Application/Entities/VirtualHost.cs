using System;
using System.Collections.Generic;
using System.Text;

namespace Pwntainer.Application.Entities
{
    public class VirtualHost : BaseAsset
    {
        public string Name { get; set; }
        public Service Service { get; set; }
        public int ServiceId { get; set; }
    }
}
