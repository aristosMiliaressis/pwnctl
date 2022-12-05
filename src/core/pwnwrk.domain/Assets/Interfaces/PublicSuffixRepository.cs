using pwnwrk.domain.Assets.ValueObjects;

namespace pwnwrk.domain.Assets.Interfaces
{
    public interface PublicSuffixRepository
    {
        static PublicSuffixRepository Instance { get; set; }

        public List<PublicSuffix> List();
    }    
}