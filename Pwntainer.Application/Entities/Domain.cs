using System;
using System.Collections.Generic;
using System.Text;

namespace Pwntainer.Application.Entities
{
    public class Domain : BaseAsset
    {
        public string Name { get; set; }
        public bool InScope { get; set; } = true;
    }
}
