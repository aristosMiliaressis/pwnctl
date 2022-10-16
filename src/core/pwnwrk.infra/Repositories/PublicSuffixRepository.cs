using pwnwrk.infra.Configuration;
using pwnwrk.domain.Interfaces;
using pwnwrk.domain.ValueObjects;

namespace pwnwrk.infra.Repositories
{
    public class PublicSuffixRepository : IPublicSuffixRepository
    {
        private string _publicSuffixDataFile = PwnContext.Config.IsTestRun
                            ? $"{AppConfig.InstallPath}/dns/public_suffix_list.dat"
                            : "/opt/wordlists/dns/public_suffix_list.dat";
        private List<PublicSuffix> _publicSuffixes;
        private static PublicSuffixRepository _singleton;

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
    }
}
