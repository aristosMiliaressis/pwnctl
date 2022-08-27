using pwnctl.core.Interfaces;

namespace pwnctl.core
{
    public static class PwnctlCoreShim
    {
        public static IPublicSuffixRepository PublicSuffixRepository { get; set; }
    }
}