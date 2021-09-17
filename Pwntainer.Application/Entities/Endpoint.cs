using System;
using System.Collections.Generic;
using System.Text;

namespace Pwntainer.Application.Entities
{
    public class Endpoint : BaseAsset
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public string Uri { get; set; }
    }
}
