using pwnwrk.domain.ValueObjects;

namespace pwnwrk.domain.Interfaces
{
    public interface IPublicSuffixRepository
    {
        public List<PublicSuffix> GetSuffixes();
    }    
}