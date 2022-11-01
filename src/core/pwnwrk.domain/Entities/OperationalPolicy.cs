using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities
{
    public class OperationalPolicy : BaseEntity<int>
    {
        public string Name { get; init; }
		public string Blacklist { get; init; }
		public string Whitelist { get; init; }
		public int? MaxAggressiveness { get; init; }
		public bool AllowActive { get; init; }

        public OperationalPolicy() {}
    }
}