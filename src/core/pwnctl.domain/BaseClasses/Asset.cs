using pwnctl.kernel.BaseClasses;

namespace pwnctl.domain.BaseClasses
{
    public abstract class Asset : Entity<string>
    {
        public abstract override string ToString();
    }
}
