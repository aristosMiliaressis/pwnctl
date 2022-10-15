using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities
{
    public class OperationalPolicy : BaseEntity<int>
    {
        public string Name { get; private init; }
		public string Blacklist { get; private init; }
		public string Whitelist { get; private init; }
		public int? MaxAggressiveness { get; private init; }
		public bool AllowActive { get; private init; }

        public OperationalPolicy() {}
    }
}