using pwnctl.domain.Interfaces;
using pwnctl.domain.ValueObjects;
using pwnctl.infra.Configuration;

namespace pwnctl.infra.Repositories
{
    public sealed class FsPublicSuffixListService : PublicSuffixListService
    {
        private static string _publicSuffixDataFile = $"{EnvironmentVariables.InstallPath}/public_suffix_list.dat";

        private List<PublicSuffix> _suffixes;

        public PublicSuffix GetSuffix(string suffix)
        {
            return List()
                .Where(s => suffix.EndsWith($".{s.Value}"))
                .OrderByDescending(s => s.Value.Length)
                .FirstOrDefault();
        }

        private List<PublicSuffix> List()
        {
            if (_suffixes == null)
            {
                _suffixes = File.ReadLines(_publicSuffixDataFile)
                    .Select(suffix => {
                        try {
                            return PublicSuffix.Create(suffix);
                        } catch {
                            return null;
                        }})
                    .Where(s => s != null)
                    .Distinct()
                    .ToList();
            }

            return _suffixes;
        }
    }
}
