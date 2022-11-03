using pwnwrk.domain.Assets.Interfaces;
using pwnwrk.domain.Assets.ValueObjects;
using pwnwrk.infra.Configuration;

namespace pwnwrk.infra.Repositories
{
    public sealed class PublicSuffixRepository : IPublicSuffixRepository
    {
        private static string _publicSuffixDataFile = $"{EnvironmentVariables.InstallPath}/public_suffix_list.dat";

        private List<PublicSuffix> _publicSuffixes;

        public List<PublicSuffix> List()
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
