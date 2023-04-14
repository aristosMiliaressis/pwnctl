using pwnctl.domain.Interfaces;
using pwnctl.domain.ValueObjects;
using pwnctl.infra.Configuration;

namespace pwnctl.infra.Repositories
{
    public sealed class FsPublicSuffixRepository : PublicSuffixRepository
    {
        private static string _publicSuffixDataFile = Path.Combine(EnvironmentVariables.INSTALL_PATH, "public_suffix_list.dat");
        private static List<PublicSuffix> _suffixes;

        public PublicSuffix GetSuffix(string suffix)
        {
            return List()
                .Where(s => suffix.EndsWith("."+s.Value))
                .OrderByDescending(s => s.Value.Length)
                .FirstOrDefault();
        }

        private List<PublicSuffix> List()
        {
            if (_suffixes == null)
                _suffixes = File.ReadLines(_publicSuffixDataFile)
                        .Distinct()
                        .Where(suf => Uri.CheckHostName(suf) == UriHostNameType.Dns)
                        .Select(suffix => PublicSuffix.Create(suffix))
                        .ToList();
            
            return _suffixes;
        }
    }
}
