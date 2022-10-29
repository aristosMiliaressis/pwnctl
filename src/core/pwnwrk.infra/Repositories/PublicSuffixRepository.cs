using pwnwrk.domain.Interfaces;
using pwnwrk.domain.ValueObjects;
using pwnwrk.infra.Configuration;

namespace pwnwrk.infra.Repositories
{
    public class PublicSuffixRepository : IPublicSuffixRepository
    {
        private static string _publicSuffixDataFile = $"{EnvironmentVariables.EfsMountPoint}/dns/public_suffix_list.dat";

        private List<PublicSuffix> _publicSuffixes;

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
