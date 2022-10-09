using pwnwrk.infra.Configuration;
using pwnwrk.domain.Interfaces;
using pwnwrk.domain.ValueObjects;

namespace pwnwrk.infra.Repositories
{
    public class PublicSuffixRepository : IPublicSuffixRepository
    {
        private List<PublicSuffix> _publicSuffixes;
        private static PublicSuffixRepository _singleton;
        private string _publicSuffixDataFile = ConfigurationManager.Config.IsTestRun
                                    ? $"{AppConfig.InstallPath}/dns/public_suffix_list.dat"
                                    : "/opt/wordlists/dns/public_suffix_list.dat";
        
        public static PublicSuffixRepository Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new PublicSuffixRepository();
    
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
