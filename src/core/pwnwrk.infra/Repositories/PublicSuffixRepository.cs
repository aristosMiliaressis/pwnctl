using pwnwrk.infra.Configuration;
using pwnwrk.domain.Interfaces;
using pwnwrk.domain.ValueObjects;

namespace pwnwrk.infra.Repositories
{
    public class PublicSuffixRepository : IPublicSuffixRepository
    {
        private static string _publicSuffixDataFile = PwnContext.Config.IsTestRun
                            ? $"./dns/public_suffix_list.dat"
                            : "/opt/wordlists/dns/public_suffix_list.dat";
        private List<PublicSuffix> _publicSuffixes= File.ReadLines(_publicSuffixDataFile)
                    .Select(suffix => PublicSuffix.Create(suffix))
                    .Distinct()
                    .ToList();

        public List<PublicSuffix> GetSuffixes()
        {
            return _publicSuffixes;
        }
    }
}
