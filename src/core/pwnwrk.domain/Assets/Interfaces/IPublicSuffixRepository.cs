using pwnwrk.domain.Assets.ValueObjects;

namespace pwnwrk.domain.Assets.Interfaces
{
    public interface IPublicSuffixRepository
    {
        static IPublicSuffixRepository Instance { get; set; }

        public List<PublicSuffix> List();
    }    
}