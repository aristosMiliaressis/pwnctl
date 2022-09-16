using pwnctl.core.ValueObjects;

namespace pwnctl.core.Interfaces
{
    public interface IPublicSuffixRepository
    {
        public string GetRegistrationDomain(string domain);
        public PublicSuffix GetPublicSuffix(string domain);
    }    
}