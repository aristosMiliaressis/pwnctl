using pwnwrk.domain.Interfaces;

namespace pwnwrk.domain
{
    public static class PwnwrkDomainShim
    {
        public static IPublicSuffixRepository PublicSuffixRepository { get; set; }
    }
}