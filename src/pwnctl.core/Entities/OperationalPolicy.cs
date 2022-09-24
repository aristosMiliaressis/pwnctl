using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities
{
    public class OperationalPolicy : BaseEntity<int>
    {
        public string Name { get; set; }
		public string Blacklist { get; set; }
		public string Whitelist { get; set; }
		public int? MaxAggressiveness { get; set; }
		public bool AllowActive { get; set; }

        public OperationalPolicy() {}
    }
}