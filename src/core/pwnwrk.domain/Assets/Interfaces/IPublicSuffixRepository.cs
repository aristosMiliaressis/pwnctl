using pwnwrk.domain.Assets.ValueObjects;

namespace pwnwrk.domain.Assets.Interfaces
{
    public interface IPublicSuffixRepository
    {
        public List<PublicSuffix> GetSuffixes();
    }    
}