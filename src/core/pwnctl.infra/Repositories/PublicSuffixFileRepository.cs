using pwnctl.domain.Interfaces;
using pwnctl.domain.ValueObjects;
using pwnctl.infra.Configuration;

namespace pwnctl.infra.Repositories
{
    public sealed class PublicSuffixFileRepository : PublicSuffixRepository
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
