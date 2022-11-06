using pwnwrk.domain.Assets.ValueObjects;
using pwnwrk.domain.Common.Interfaces;

namespace pwnwrk.domain.Assets.Interfaces
{
    public interface IPublicSuffixRepository : IAmbientService
    {
        public List<PublicSuffix> List();
    }    
}