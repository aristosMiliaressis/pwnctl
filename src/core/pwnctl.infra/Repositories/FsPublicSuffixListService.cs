using pwnctl.domain.Services;
using pwnctl.domain.ValueObjects;
using pwnctl.infra.Configuration;

namespace pwnctl.infra.Repositories
{
    public sealed class FsPublicSuffixListService : PublicSuffixListService
    {
        private static string _publicSuffixDataFile = $"{EnvironmentVariables.InstallPath}/public_suffix_list.dat";

        private List<PublicSuffix> _publicSuffixes;

        public PublicSuffix GetPublicSuffix(string name)
        {
            return List()
                .Where(suffix => name.EndsWith($".{suffix.Suffix}"))
                .OrderByDescending(s => s.Suffix.Length)
                .FirstOrDefault();
        }

        private List<PublicSuffix> List()
        {
            if (_publicSuffixes == null)
            {
                _publicSuffixes = File.ReadLines(_publicSuffixDataFile)
                    .Select(suffix => 
                    {
                        try
                        {
                            return PublicSuffix.Create(suffix);
                        } catch {
                            return null;
                        }
                    })
                    .Where(s => s != null)
                    .Distinct()
                    .ToList();
            }

            return _publicSuffixes;
        }
    }
}
