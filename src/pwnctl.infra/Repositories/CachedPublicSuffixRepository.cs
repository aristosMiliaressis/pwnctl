using pwnctl.infra.Configuration;
using pwnctl.core.Interfaces;
using pwnctl.core.ValueObjects;

// TODO: look into Nager.PublicSuffix && use in-memory cache

namespace pwnctl.infra.Repositories
{
    public class CachedPublicSuffixRepository : IPublicSuffixRepository
    {
        private List<PublicSuffix> _publicSuffixes;
        private static CachedPublicSuffixRepository _singleton;
        private string _publicSuffixDataFile = EnvironmentVariables.PWNCTL_TEST 
                                    ? $"{EnvironmentVariables.PWNCTL_INSTALL_PATH}/dns/public_suffix_list.dat"
                                    : "/opt/wordlists/dns/public_suffix_list.dat";
        
        public static CachedPublicSuffixRepository Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new CachedPublicSuffixRepository();
    
                return _singleton;
            }
        }
        
        public List<PublicSuffix> GetSuffixes()
        {
            if (_publicSuffixes == null)
            {
                _publicSuffixes = File.ReadLines(_publicSuffixDataFile)
                    .Select(suffix => PublicSuffix.Create(suffix))
                    .Distinct()
                    .ToList();
            }
    
            return _publicSuffixes;
        }

        public string GetRegistrationDomain(string domain)
        {
            var suffix = GetPublicSuffix(domain);
            if (suffix == null)
                return null;

            return domain
                    .Substring(0, domain.Length - suffix.Suffix.Length - 1)
                    .Split(".")
                    .Last() + "." + suffix.Suffix;
        }

        public PublicSuffix GetPublicSuffix(string domain)
        {
            return GetSuffixes()
                         .Where(suffix => domain.EndsWith($".{suffix.Suffix}"))
                         .OrderByDescending(s => s.Suffix.Length)
                         .FirstOrDefault();
        }
    }
}
