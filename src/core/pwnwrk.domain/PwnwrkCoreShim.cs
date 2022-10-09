using pwnwrk.domain.Interfaces;

namespace pwnwrk.domain
{
    public static class PwnwrkCoreShim
    {
        public static IPublicSuffixRepository PublicSuffixRepository { get; set; }
    }
}