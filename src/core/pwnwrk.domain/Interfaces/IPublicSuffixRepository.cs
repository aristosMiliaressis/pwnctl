using pwnwrk.domain.ValueObjects;

namespace pwnwrk.domain.Interfaces
{
    public interface IPublicSuffixRepository
    {
        public string GetRegistrationDomain(string domain);
        public PublicSuffix GetPublicSuffix(string domain);
    }    
}