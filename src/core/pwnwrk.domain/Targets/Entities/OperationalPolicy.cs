using pwnwrk.domain.Common.BaseClasses;

namespace pwnwrk.domain.Targets.Entities
{
    public sealed class OperationalPolicy : Entity<int>
    {
        public string Name { get; init; }
		public string Blacklist { get; init; }
		public string Whitelist { get; init; }
		public int? MaxAggressiveness { get; init; }
		public bool AllowActive { get; init; }

        public OperationalPolicy() {}
    }
}